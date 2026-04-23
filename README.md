# Load Balancing Algorithms Benchmark

A .NET-based distributed systems project that simulates a **Load Manager** and multiple **Node Servers** to benchmark different load balancing algorithms under concurrent stress testing.

## Overview

This project demonstrates how a load balancer distributes incoming requests across multiple backend nodes. Each node executes the same CPU-intensive mathematical function, which creates realistic computational load for testing and comparison.

The system is designed to evaluate the behavior and performance of several load balancing strategies using Docker containers and ApacheBench.

## Architecture

The system consists of:

* **LoadManager**: Receives incoming requests, selects a backend node using the chosen algorithm, forwards the request, and collects metrics.
* **NodeServer**: Executes the mathematical function `H(n)` and returns the result along with node metadata.
* **Docker Compose**: Runs one LoadManager container and five NodeServer containers.
* **ApacheBench (ab)**: Sends high-volume concurrent requests for benchmarking.

### High-Level Flow

1. The client sends a request to the LoadManager.
2. The LoadManager selects a node using the requested algorithm.
3. The request is forwarded to the selected NodeServer.
4. The NodeServer computes the function and returns the response.
5. The LoadManager records metrics and returns the backend response.

## Implemented Algorithms

The following algorithms are currently supported:

- **Round Robin** (`rr`)
- **Random Selection** (`random`)
- **Weighted Round Robin** (`wrr`)
- **Least Connections** (`lc`)
- **Hash-Based Selection** (`hash`)

## Backend Workload

Each NodeServer evaluates a CPU-intensive function of the form:

`H(n) = Σ from i = 1 to n × 10^6 of ( sqrt(i) × sin(i) / ln(i + 1) )`

This makes each request computationally expensive enough to observe meaningful load balancing behavior during stress testing.

## Project Structure

```text
LoadBalancingProject/
├── LoadBalancingProject.sln
├── docker-compose.yml
├── README.md
├── run-tests.sh
├── sample_docker_stats.sh
├── compare_results.py
├── results/
├── LoadManager/
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   ├── Strategies/
│   ├── Program.cs
│   └── Dockerfile
└── NodeServer/
    ├── Controllers/
    ├── Models/
    ├── Services/
    ├── Program.cs
    └── Dockerfile
```

## Main Endpoints

### LoadManager

- `GET /LoadBalancer/cal?n=1&algo=rr`
- `GET /Metrics`
- `POST /Metrics/reset`
- `GET /Nodes`

### NodeServer

- `GET /cal?n=1`
- `GET /health`

## Technologies Used

- **.NET 8 Web API**
- **Docker & Docker Compose**
- **ApacheBench (ab)**
- **C#**
- **REST APIs**

## How to Run

### 1. Clone the repository

```bash
git clone https://github.com/Mohamed-software-engineer/Load_Balancing.git
cd Load_Balancing
```

### 2. Run the system with Docker Compose

```bash
docker compose up --build
```

If your environment uses the legacy command:

```bash
docker-compose up --build
```

This starts:

- `loadmanager` on port `8090`
- `node1` on port `5001`
- `node2` on port `5002`
- `node3` on port `5003`
- `node4` on port `5004`
- `node5` on port `5005`

## Swagger URLs

If Swagger is enabled in `Program.cs`, you can access:

- `http://localhost:8090/swagger` for LoadManager
- `http://localhost:5001/swagger` for Node 1
- `http://localhost:5002/swagger` for Node 2
- `http://localhost:5003/swagger` for Node 3
- `http://localhost:5004/swagger` for Node 4
- `http://localhost:5005/swagger` for Node 5

## Manual Testing

### Test a single node

```bash
curl "http://localhost:5001/health"
curl "http://localhost:5001/cal?n=1"
```

### Test through the LoadManager

```bash
curl "http://localhost:8090/LoadBalancer/cal?n=1&algo=rr"
curl "http://localhost:8090/LoadBalancer/cal?n=1&algo=random"
curl "http://localhost:8090/LoadBalancer/cal?n=1&algo=wrr"
curl "http://localhost:8090/LoadBalancer/cal?n=1&algo=lc"
curl "http://localhost:8090/LoadBalancer/cal?n=1&algo=hash"
```

### Reset and read metrics

```bash
curl -X POST http://localhost:8090/Metrics/reset
curl http://localhost:8090/Metrics
```

## Stress Testing

Make sure ApacheBench is installed:

```bash
sudo apt update
sudo apt install apache2-utils
```

## Run the Full Project Through the Benchmark Script

The project can also be executed through the benchmark pipeline script, which automates the full workflow:

- starts Docker containers
- waits for services to become available
- resets metrics before each algorithm
- runs stress tests for all algorithms
- collects real Docker CPU and memory statistics
- saves raw benchmark outputs
- generates final comparison summaries

Run:

```bash
chmod +x run-tests.sh
chmod +x sample_docker_stats.sh
./run-tests.sh
```

### Example benchmark

```bash
ab -n 10000 -c 200 "http://localhost:8090/LoadBalancer/cal?n=1&algo=rr"
```

### Benchmark all algorithms

```bash
ab -n 10000 -c 200 "http://localhost:8090/LoadBalancer/cal?n=1&algo=random"
ab -n 10000 -c 200 "http://localhost:8090/LoadBalancer/cal?n=1&algo=rr"
ab -n 10000 -c 200 "http://localhost:8090/LoadBalancer/cal?n=1&algo=wrr"
ab -n 10000 -c 200 "http://localhost:8090/LoadBalancer/cal?n=1&algo=lc"
ab -n 10000 -c 200 "http://localhost:8090/LoadBalancer/cal?n=1&algo=hash"
```

## Automated Benchmark Pipeline

The project includes an automated benchmark pipeline using:

- `run-tests.sh`
- `sample_docker_stats.sh`
- `compare_results.py`

### What it does

1. Starts Docker containers
2. Waits for services to become available
3. Resets metrics before each algorithm
4. Runs ApacheBench for each algorithm
5. Collects real Docker CPU and memory statistics
6. Saves raw benchmark outputs
7. Generates final summary files

### Run the full benchmark

```bash
chmod +x run-tests.sh
chmod +x sample_docker_stats.sh
./run-tests.sh
```

## Generated Outputs

For each algorithm, the benchmark pipeline generates three raw output files:

- `*-ab.txt` → ApacheBench raw output
- `*-cpu.csv` → Docker CPU and memory samples collected during the test
- `*-metrics.json` → LoadManager metrics snapshot after the test

Examples:

- `rr-ab.txt`
- `rr-cpu.csv`
- `rr-metrics.json`

The final comparison files are:

- `results/summary.txt`
- `results/summary.csv`

These outputs make it easier to review raw benchmark data and compare all algorithms in a single summarized report.

## Metrics Collected

The benchmark compares algorithms using:

- total execution time
- requests per second
- mean time per request
- failed requests
- per-node request distribution
- active connections
- real Docker CPU usage per container
- memory usage samples

## Best Algorithm Analysis

The comparison pipeline automatically identifies:

- **Fastest Algorithm**
- **Highest Throughput**
- **Lowest Failed Requests**
- **Lowest LoadManager CPU**
- **Best Load Distribution**
- **Best Overall Algorithm**

The best overall result is based on a weighted comparison of:

- execution time
- throughput
- failure rate
- load manager CPU usage
- fairness of load distribution

## Real CPU Measurement

The project measures real container CPU usage using Docker statistics during benchmark execution, which makes the comparison more meaningful than relying only on total execution time.

## Notes

- `Round Robin` is the simplest and most balanced algorithm when all nodes have similar capacity.
- `Weighted Round Robin` gives more traffic to nodes with higher configured weights.
- `Least Connections` chooses the node with fewer active requests.
- `Hash-Based Selection` maps requests to nodes using a hash-derived index.
- For heavy stress testing, start with `n=1` before increasing the workload parameter.

## Future Improvements

- Read node configuration from `appsettings.json` instead of hardcoding
- Add real health checks between LoadManager and NodeServers
- Add retry and fallback logic when a node fails
- Add response-time-based balancing
- Generate charts automatically from benchmark results
- Export results to Excel automatically
- Build a live dashboard for visualization
- Add smarter algorithm recommendation based on workload profile
  
## Author

**Mohamed Saad**

- GitHub: [Mohamed-software-engineer](https://github.com/Mohamed-software-engineer)
- LinkedIn: [mohamed-saad-engineering](https://www.linkedin.com/in/mohamed-saad-engineering)
