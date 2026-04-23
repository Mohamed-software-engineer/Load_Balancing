# Load Balancing Algorithms Benchmark

A .NET-based distributed systems project that simulates a **Load Manager** and multiple **Node Servers** to benchmark different load balancing algorithms under concurrent stress testing.

## Overview

This project demonstrates how a load balancer distributes incoming requests across multiple backend nodes. Each backend node executes the same CPU-intensive mathematical function, which creates realistic computational load for testing and comparison.

The system is designed not only to implement multiple load balancing strategies, but also to **benchmark** them under heavy concurrent traffic using Docker containers, ApacheBench, automated scripts, and result analysis tools.

## Architecture

The system consists of:

- **LoadManager**: Receives incoming client requests, selects a backend node using the requested algorithm, forwards the request, and collects metrics.
- **NodeServer**: Executes the mathematical function `H(n)` and returns the result along with node metadata.
- **Docker Compose**: Runs one LoadManager container and five NodeServer containers.
- **ApacheBench (ab)**: Sends high-volume concurrent requests for benchmarking.
- **Benchmark Scripts**: Automate testing, CPU sampling, and result comparison.

### High-Level Flow

1. The client sends a request to the LoadManager.
2. The LoadManager selects a backend node using the chosen algorithm.
3. The request is forwarded to the selected NodeServer.
4. The NodeServer computes the workload and returns the result.
5. The LoadManager records metrics and returns the backend response to the client.

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
│   ├── rr-ab.txt
│   ├── rr-cpu.csv
│   ├── rr-metrics.json
│   ├── random-ab.txt
│   ├── random-cpu.csv
│   ├── random-metrics.json
│   ├── wrr-ab.txt
│   ├── wrr-cpu.csv
│   ├── wrr-metrics.json
│   ├── lc-ab.txt
│   ├── lc-cpu.csv
│   ├── lc-metrics.json
│   ├── hash-ab.txt
│   ├── hash-cpu.csv
│   ├── hash-metrics.json
│   ├── summary.txt
│   └── summary.csv
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
    
    
Main Endpoints
LoadManager
GET /LoadBalancer/cal?n=1&algo=rr
GET /Metrics
POST /Metrics/reset
GET /Nodes
NodeServer
GET /cal?n=1
GET /health
Technologies Used
.NET 8 Web API
C#
Docker
Docker Compose
ApacheBench (ab)
Python for result aggregation and comparison
Shell scripting for automated benchmarking
REST APIs
How to Run
1. Clone the repository
git clone https://github.com/Mohamed-software-engineer/Load_Balancing.git
cd Load_Balancing
2. Run the system with Docker Compose

If your environment supports the modern Docker Compose plugin:

docker compose up --build

If your environment uses the legacy command:

docker-compose up --build

This starts:

loadmanager on port 8090
node1 on port 5001
node2 on port 5002
node3 on port 5003
node4 on port 5004
node5 on port 5005
Swagger URLs

If Swagger is enabled in Program.cs, you can access:

http://localhost:8090/swagger for LoadManager
http://localhost:5001/swagger for Node 1
http://localhost:5002/swagger for Node 2
http://localhost:5003/swagger for Node 3
http://localhost:5004/swagger for Node 4
http://localhost:5005/swagger for Node 5
Manual Testing
Test a single node
curl "http://localhost:5001/health"
curl "http://localhost:5001/cal?n=1"
Test through the LoadManager
curl "http://localhost:8090/LoadBalancer/cal?n=1&algo=rr"
curl "http://localhost:8090/LoadBalancer/cal?n=1&algo=random"
curl "http://localhost:8090/LoadBalancer/cal?n=1&algo=wrr"
curl "http://localhost:8090/LoadBalancer/cal?n=1&algo=lc"
curl "http://localhost:8090/LoadBalancer/cal?n=1&algo=hash"
Reset and read metrics
curl -X POST http://localhost:8090/Metrics/reset
curl http://localhost:8090/Metrics
Stress Testing

Make sure ApacheBench is installed:

sudo apt update
sudo apt install apache2-utils
Example benchmark
ab -n 10000 -c 200 "http://localhost:8090/LoadBalancer/cal?n=1&algo=rr"
Benchmark all algorithms manually
ab -n 10000 -c 200 "http://localhost:8090/LoadBalancer/cal?n=1&algo=random"
ab -n 10000 -c 200 "http://localhost:8090/LoadBalancer/cal?n=1&algo=rr"
ab -n 10000 -c 200 "http://localhost:8090/LoadBalancer/cal?n=1&algo=wrr"
ab -n 10000 -c 200 "http://localhost:8090/LoadBalancer/cal?n=1&algo=lc"
ab -n 10000 -c 200 "http://localhost:8090/LoadBalancer/cal?n=1&algo=hash"
Automated Benchmark Pipeline

The project includes an automated benchmark pipeline that performs the full comparison process.

Files involved
run-tests.sh
Main script that runs all benchmark scenarios.
sample_docker_stats.sh
Collects real Docker CPU and memory usage during each test.
compare_results.py
Reads the raw outputs and produces final summaries.
What run-tests.sh does
Starts Docker containers
Waits for services to become available
Resets metrics before each algorithm
Runs ApacheBench for each algorithm
Collects Docker CPU and memory statistics
Saves raw benchmark outputs
Calls compare_results.py
Generates final summaries
Run the full benchmark
chmod +x run-tests.sh
chmod +x sample_docker_stats.sh
./run-tests.sh
Output Files

Each algorithm produces three raw output files:

*-ab.txt → ApacheBench output
*-cpu.csv → Docker CPU and memory samples
*-metrics.json → LoadManager metrics snapshot

Examples:

rr-ab.txt
rr-cpu.csv
rr-metrics.json

The final comparison files are:

results/summary.txt
results/summary.csv
Metrics Collected

The LoadManager tracks:

Total requests
Successful requests
Failed requests
Requests per algorithm
Per-node total requests
Per-node failed requests
Active connections per node

The benchmark pipeline also collects:

ApacheBench execution time
Requests per second
Mean time per request
Failed requests from ApacheBench
Real Docker CPU usage per container
Docker memory usage samples
Per-node request distribution
Best algorithm analysis

Best Algorithm Analysis

The comparison script automatically analyzes benchmark outputs and identifies:

Fastest Algorithm
Highest Throughput
Lowest Failed Requests
Lowest LoadManager CPU
Best Load Distribution
Best Overall Algorithm

The Best Overall result is determined using a weighted score based on:

Total execution time
Requests per second
Failure rate
LoadManager CPU usage
Distribution fairness

This means the project is not only a benchmarking system, but also a recommendation-oriented evaluation system for load balancing algorithms under the tested workload.

Real CPU Measurement

The project measures real container CPU usage using Docker statistics during benchmark execution.

This gives realistic visibility into:

loadmanager CPU usage
node1 CPU usage
node2 CPU usage
node3 CPU usage
node4 CPU usage
node5 CPU usage

This makes the comparison more meaningful than relying only on total execution time.

Notes
Round Robin is the simplest and most balanced algorithm when all nodes have similar capacity.
Weighted Round Robin gives more traffic to nodes with higher configured weights.
Least Connections chooses the node with fewer active requests.
Hash-Based Selection maps requests to nodes using a hash-derived index.
For heavy stress testing, start with n=1 before increasing the workload parameter.
If your system does not support docker compose, use docker-compose instead.
Future Improvements

Potential improvements for later versions:

Read node configuration from appsettings.json instead of hardcoding
Add real health checks between LoadManager and NodeServers
Add response-time-based balancing
Add retry and fallback logic when a node fails
Export benchmark results to Excel automatically
Generate charts automatically from summary.csv
Build a dashboard for live visualization
Add a smarter workload-aware algorithm recommendation engine
CV-Friendly Summary

Built a .NET-based load balancing simulation system with a Load Manager and multiple backend Node Servers, implementing and benchmarking Round Robin, Weighted Round Robin, Random, Least Connections, and Hash-based selection under concurrent stress testing using Docker, ApacheBench, automated benchmarking scripts, and real container CPU monitoring.

