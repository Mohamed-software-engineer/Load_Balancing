import csv
import json
import os
import re
from collections import defaultdict
from math import sqrt

RESULTS_DIR = "results"
ALGORITHMS = ["rr", "random", "wrr", "lc", "hash"]


def parse_ab_file(path: str):
    data = {
        "time_taken_sec": 0.0,
        "requests_per_sec": 0.0,
        "time_per_request_ms": 0.0,
        "failed_requests": 0
    }

    if not os.path.exists(path):
        return data

    with open(path, "r", encoding="utf-8", errors="ignore") as f:
        content = f.read()

    patterns = {
        "time_taken_sec": r"Time taken for tests:\s+([0-9.]+)",
        "requests_per_sec": r"Requests per second:\s+([0-9.]+)",
        "time_per_request_ms": r"Time per request:\s+([0-9.]+)\s+\[ms\]\s+\(mean\)",
        "failed_requests": r"Failed requests:\s+([0-9]+)"
    }

    for key, pattern in patterns.items():
        match = re.search(pattern, content)
        if match:
            value = match.group(1)
            if key == "failed_requests":
                data[key] = int(value)
            else:
                data[key] = float(value)

    return data


def parse_metrics_file(path: str):
    data = {
        "total_requests": 0,
        "successful_requests": 0,
        "failed_requests_metrics": 0,
        "node_distribution_map": {},
        "node_distribution_text": ""
    }

    if not os.path.exists(path):
        return data

    with open(path, "r", encoding="utf-8") as f:
        obj = json.load(f)

    data["total_requests"] = obj.get("totalRequests", 0)
    data["successful_requests"] = obj.get("successfulRequests", 0)
    data["failed_requests_metrics"] = obj.get("failedRequests", 0)

    nodes = obj.get("nodes", [])
    distribution_parts = []
    distribution_map = {}

    for node in nodes:
        node_id = node.get("nodeId", "")
        total = node.get("totalRequests", 0)
        distribution_map[node_id] = total
        distribution_parts.append(f"{node_id}:{total}")

    data["node_distribution_map"] = distribution_map
    data["node_distribution_text"] = " | ".join(distribution_parts)
    return data


def parse_cpu_value(cpu_str: str) -> float:
    cpu_str = cpu_str.strip().replace("%", "")
    try:
        return float(cpu_str)
    except ValueError:
        return 0.0


def parse_cpu_file(path: str):
    if not os.path.exists(path):
        return {
            "avg_cpu_by_container_map": {},
            "max_cpu_by_container_map": {},
            "avg_cpu_by_container_text": "",
            "max_cpu_by_container_text": ""
        }

    cpu_values = defaultdict(list)

    with open(path, "r", encoding="utf-8", errors="ignore") as f:
        reader = csv.DictReader(f)
        for row in reader:
            container = row["container"].strip()
            cpu = parse_cpu_value(row["cpu_percent"])
            cpu_values[container].append(cpu)

    avg_map = {}
    max_map = {}
    avg_parts = []
    max_parts = []

    for container, values in sorted(cpu_values.items()):
        if not values:
            continue
        avg_cpu = sum(values) / len(values)
        max_cpu = max(values)

        avg_map[container] = avg_cpu
        max_map[container] = max_cpu

        avg_parts.append(f"{container}:{avg_cpu:.2f}%")
        max_parts.append(f"{container}:{max_cpu:.2f}%")

    return {
        "avg_cpu_by_container_map": avg_map,
        "max_cpu_by_container_map": max_map,
        "avg_cpu_by_container_text": " | ".join(avg_parts),
        "max_cpu_by_container_text": " | ".join(max_parts),
    }


def compute_balance_score(distribution_map: dict) -> float:
    """
    كل ما النتيجة كانت أقل، التوزيع أفضل.
    هنستخدم standard deviation على counts.
    """
    values = list(distribution_map.values())
    if not values:
        return float("inf")

    mean = sum(values) / len(values)
    variance = sum((x - mean) ** 2 for x in values) / len(values)
    return sqrt(variance)


def normalize_inverse(value, min_value, max_value):
    """
    للأشياء اللي الأقل فيها أفضل: time, failures, cpu, balance score
    """
    if max_value == min_value:
        return 1.0
    return (max_value - value) / (max_value - min_value)


def normalize_direct(value, min_value, max_value):
    """
    للأشياء اللي الأعلى فيها أفضل: requests/sec
    """
    if max_value == min_value:
        return 1.0
    return (value - min_value) / (max_value - min_value)


rows = []

for algo in ALGORITHMS:
    ab_data = parse_ab_file(os.path.join(RESULTS_DIR, f"{algo}-ab.txt"))
    metrics_data = parse_metrics_file(os.path.join(RESULTS_DIR, f"{algo}-metrics.json"))
    cpu_data = parse_cpu_file(os.path.join(RESULTS_DIR, f"{algo}-cpu.csv"))

    loadmanager_avg_cpu = cpu_data["avg_cpu_by_container_map"].get("loadmanager", 0.0)

    balance_score = compute_balance_score(metrics_data["node_distribution_map"])

    row = {
        "algorithm": algo,
        "ab_time_taken_sec": ab_data["time_taken_sec"],
        "ab_requests_per_sec": ab_data["requests_per_sec"],
        "ab_time_per_request_ms": ab_data["time_per_request_ms"],
        "ab_failed_requests": ab_data["failed_requests"],
        "metrics_total_requests": metrics_data["total_requests"],
        "metrics_successful_requests": metrics_data["successful_requests"],
        "metrics_failed_requests": metrics_data["failed_requests_metrics"],
        "node_distribution": metrics_data["node_distribution_text"],
        "avg_cpu_by_container": cpu_data["avg_cpu_by_container_text"],
        "max_cpu_by_container": cpu_data["max_cpu_by_container_text"],
        "loadmanager_avg_cpu": loadmanager_avg_cpu,
        "balance_score": balance_score,
    }

    rows.append(row)


# ---------- Best-of calculations ----------

fastest = min(rows, key=lambda r: r["ab_time_taken_sec"])
highest_throughput = max(rows, key=lambda r: r["ab_requests_per_sec"])
lowest_failures = min(rows, key=lambda r: r["ab_failed_requests"])
lowest_loadmanager_cpu = min(rows, key=lambda r: r["loadmanager_avg_cpu"])
best_balanced = min(rows, key=lambda r: r["balance_score"])


# ---------- Overall score ----------
times = [r["ab_time_taken_sec"] for r in rows]
throughputs = [r["ab_requests_per_sec"] for r in rows]
failures = [r["ab_failed_requests"] for r in rows]
loadmanager_cpus = [r["loadmanager_avg_cpu"] for r in rows]
balances = [r["balance_score"] for r in rows]

min_time, max_time = min(times), max(times)
min_thr, max_thr = min(throughputs), max(throughputs)
min_fail, max_fail = min(failures), max(failures)
min_cpu, max_cpu = min(loadmanager_cpus), max(loadmanager_cpus)
min_bal, max_bal = min(balances), max(balances)

for row in rows:
    score_time = normalize_inverse(row["ab_time_taken_sec"], min_time, max_time)
    score_thr = normalize_direct(row["ab_requests_per_sec"], min_thr, max_thr)
    score_fail = normalize_inverse(row["ab_failed_requests"], min_fail, max_fail)
    score_cpu = normalize_inverse(row["loadmanager_avg_cpu"], min_cpu, max_cpu)
    score_balance = normalize_inverse(row["balance_score"], min_bal, max_bal)

    # الأوزان
    overall_score = (
        score_time * 0.35 +
        score_thr * 0.30 +
        score_fail * 0.15 +
        score_cpu * 0.10 +
        score_balance * 0.10
    )

    row["overall_score"] = round(overall_score, 4)

best_overall = max(rows, key=lambda r: r["overall_score"])


# ---------- Save CSV ----------
csv_path = os.path.join(RESULTS_DIR, "summary.csv")
with open(csv_path, "w", newline="", encoding="utf-8") as f:
    writer = csv.DictWriter(f, fieldnames=list(rows[0].keys()))
    writer.writeheader()
    writer.writerows(rows)


# ---------- Save TXT summary ----------
txt_path = os.path.join(RESULTS_DIR, "summary.txt")
with open(txt_path, "w", encoding="utf-8") as f:
    f.write("LOAD BALANCING BENCHMARK SUMMARY\n")
    f.write("=" * 100 + "\n\n")

    for row in rows:
        f.write(f"Algorithm               : {row['algorithm']}\n")
        f.write(f"AB Time Taken (sec)     : {row['ab_time_taken_sec']}\n")
        f.write(f"Requests / sec          : {row['ab_requests_per_sec']}\n")
        f.write(f"Mean Time / Request     : {row['ab_time_per_request_ms']} ms\n")
        f.write(f"AB Failed Requests      : {row['ab_failed_requests']}\n")
        f.write(f"Metrics Total           : {row['metrics_total_requests']}\n")
        f.write(f"Metrics Success         : {row['metrics_successful_requests']}\n")
        f.write(f"Metrics Failed          : {row['metrics_failed_requests']}\n")
        f.write(f"Node Distribution       : {row['node_distribution']}\n")
        f.write(f"Average CPU per Node    : {row['avg_cpu_by_container']}\n")
        f.write(f"Maximum CPU per Node    : {row['max_cpu_by_container']}\n")
        f.write(f"LoadManager Avg CPU     : {row['loadmanager_avg_cpu']:.2f}%\n")
        f.write(f"Balance Score           : {row['balance_score']:.4f}\n")
        f.write(f"Overall Score           : {row['overall_score']}\n")
        f.write("-" * 100 + "\n")

    f.write("\n")
    f.write("BEST ALGORITHM ANALYSIS\n")
    f.write("=" * 100 + "\n\n")
    f.write(f"Fastest Algorithm         : {fastest['algorithm']} ({fastest['ab_time_taken_sec']} sec)\n")
    f.write(f"Highest Throughput        : {highest_throughput['algorithm']} ({highest_throughput['ab_requests_per_sec']} req/sec)\n")
    f.write(f"Lowest Failed Requests    : {lowest_failures['algorithm']} ({lowest_failures['ab_failed_requests']} failures)\n")
    f.write(f"Lowest LoadManager CPU    : {lowest_loadmanager_cpu['algorithm']} ({lowest_loadmanager_cpu['loadmanager_avg_cpu']:.2f}%)\n")
    f.write(f"Best Load Distribution    : {best_balanced['algorithm']} (balance score = {best_balanced['balance_score']:.4f})\n")
    f.write(f"Best Overall              : {best_overall['algorithm']} (overall score = {best_overall['overall_score']})\n")

print(f"Summary written to {txt_path}")
print(f"CSV written to {csv_path}")
print("\nBest Algorithm Analysis:")
print(f"- Fastest: {fastest['algorithm']}")
print(f"- Highest Throughput: {highest_throughput['algorithm']}")
print(f"- Lowest Failures: {lowest_failures['algorithm']}")
print(f"- Lowest LoadManager CPU: {lowest_loadmanager_cpu['algorithm']}")
print(f"- Best Balanced: {best_balanced['algorithm']}")
print(f"- Best Overall: {best_overall['algorithm']}")