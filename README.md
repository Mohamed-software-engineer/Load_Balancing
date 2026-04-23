# Load Balancing Algorithms Benchmark

A .NET 8 project that simulates a **Load Manager** and multiple **Node Servers** to implement and benchmark different load balancing algorithms under concurrent stress testing.

## Features

- 5 implemented load balancing algorithms
- .NET 8 Web API architecture
- Dockerized Load Manager + 5 backend nodes
- Stress testing with ApacheBench
- Automated benchmark pipeline
- Real Docker CPU monitoring
- Final comparison report with best algorithm analysis

## Implemented Algorithms

- **Round Robin** (`rr`)
- **Random Selection** (`random`)
- **Weighted Round Robin** (`wrr`)
- **Least Connections** (`lc`)
- **Hash-Based Selection** (`hash`)

## Architecture

- **LoadManager**
  - receives client requests
  - selects a backend node using the selected algorithm
  - forwards the request
  - collects metrics
  - returns the backend response

- **NodeServer**
  - executes the CPU-intensive workload
  - returns the result with node metadata

## Request Flow

```text
Client
  ↓
LoadManager
  ↓
Selected NodeServer
  ↓
LoadManager
  ↓
Client
Backend Workload

Each NodeServer computes the following CPU-intensive function:

H(n) = Σ from i = 1 to n × 10^6 of ( sqrt(i) × sin(i) / ln(i + 1) )

This creates enough computational load to make benchmarking meaningful.

Project Structure
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
Main Endpoints
LoadManager
GET /LoadBalancer/cal?n=1&algo=rr
GET /Metrics
POST /Metrics/reset
GET /Nodes
NodeServer
GET /cal?n=1
GET /health
Tech Stack
.NET 8 Web API
C#
Docker
Docker Compose
ApacheBench
Python for benchmark result aggregation
Shell scripting
Run the Project
Clone the repository
git clone https://github.com/Mohamed-software-engineer/Load_Balancing.git
cd Load_Balancing
Start the containers
docker compose up --build

If your machine uses the legacy command:

docker-compose up --build
Services
loadmanager → localhost:8090
node1 → localhost:5001
node2 → localhost:5002
node3 → localhost:5003
node4 → localhost:5004
node5 → localhost:5005
Manual Testing
Test a node directly
curl "http://localhost:5001/health"
curl "http://localhost:5001/cal?n=1"
Test through the LoadManager
curl "http://localhost:8090/LoadBalancer/cal?n=1&algo=rr"
curl "http://localhost:8090/LoadBalancer/cal?n=1&algo=random"
curl "http://localhost:8090/LoadBalancer/cal?n=1&algo=wrr"
curl "http://localhost:8090/LoadBalancer/cal?n=1&algo=lc"
curl "http://localhost:8090/LoadBalancer/cal?n=1&algo=hash"
Metrics
curl -X POST http://localhost:8090/Metrics/reset
curl http://localhost:8090/Metrics
Stress Testing

Install ApacheBench if needed:

sudo apt update
sudo apt install apache2-utils

Example:

ab -n 10000 -c 200 "http://localhost:8090/LoadBalancer/cal?n=1&algo=rr"
Automated Benchmark Pipeline

The project includes a full benchmark pipeline using:

run-tests.sh
sample_docker_stats.sh
compare_results.py
Run the full benchmark
chmod +x run-tests.sh
chmod +x sample_docker_stats.sh
./run-tests.sh
Generated Outputs

For each algorithm, the pipeline generates:

*-ab.txt → ApacheBench raw output
*-cpu.csv → Docker CPU and memory samples
*-metrics.json → LoadManager metrics snapshot

Final outputs:

results/summary.txt
results/summary.csv
Metrics Compared

The benchmark compares algorithms using:

total execution time
requests per second
mean time per request
failed requests
per-node request distribution
active connections
real Docker CPU usage per container
Best Algorithm Analysis

The comparison pipeline automatically identifies:

Fastest Algorithm
Highest Throughput
Lowest Failed Requests
Lowest LoadManager CPU
Best Load Distribution
Best Overall Algorithm

The best overall result is based on a weighted comparison of:

execution time
throughput
failure rate
load manager CPU usage
fairness of load distribution
Why This Project Matters

This project combines:

distributed systems concepts
backend engineering
algorithm implementation
automated benchmarking
Docker-based orchestration
performance analysis

It is both an academic project and a strong portfolio project.

Future Improvements
read node configuration from appsettings.json
add real health checks and automatic node availability updates
add retry/fallback when a node fails
add response-time-based balancing
generate charts automatically from benchmark results
export results to Excel
build a live dashboard
add smarter algorithm recommendation based on workload profile
Summary

Built a .NET-based load balancing simulation system with a Load Manager and multiple backend Node Servers, implementing and benchmarking Round Robin, Weighted Round Robin, Random, Least Connections, and Hash-based selection under concurrent stress testing using Docker, ApacheBench, real container CPU monitoring, and automated result analysis

## Author

**Mohamed Saad**

- GitHub: [Mohamed-software-engineer](https://github.com/Mohamed-software-engineer)
- LinkedIn: [mohamed-saad-engineering](https://www.linkedin.com/in/mohamed-saad-engineering)
