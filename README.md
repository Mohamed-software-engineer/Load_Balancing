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

* **Round Robin** (`rr`)
* **Random Selection** (`random`)
* **Weighted Round Robin** (`wrr`)
* **Least Connections** (`lc`)
* **Hash-Based Selection** (`hash`)

## Backend Workload

Each NodeServer evaluates a CPU-intensive function of the form:

`H(n) = Σ from i = 1 to n × 10^6 of ( sqrt(i) × sin(i) / ln(i + 1) )`

This makes each request computationally expensive enough to observe meaningful load balancing behavior during stress testing.

## Project Structure

```text
LoadBalancingProject/
├── LoadBalancingProject.sln
├── docker-compose.yml
├── run-tests.sh
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

* `GET /LoadBalancer/cal?n=1&algo=rr`
* `GET /Metrics`
* `POST /Metrics/reset`
* `GET /Nodes`

### NodeServer

* `GET /cal?n=1`
* `GET /health`

## Technologies Used

* **.NET 8 Web API**
* **Docker & Docker Compose**
* **ApacheBench (ab)**
* **C#**
* **REST APIs**

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

This starts:

* `loadmanager` on port `8090`
* `node1` on port `5001`
* `node2` on port `5002`
* `node3` on port `5003`
* `node4` on port `5004`
* `node5` on port `5005`

## Swagger URLs

If Swagger is enabled in `Program.cs`, you can access:

* `http://localhost:8090/swagger` for LoadManager
* `http://localhost:5001/swagger` for Node 1
* `http://localhost:5002/swagger` for Node 2
* `http://localhost:5003/swagger` for Node 3
* `http://localhost:5004/swagger` for Node 4
* `http://localhost:5005/swagger` for Node 5

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

## Automated Benchmark Script

The project can include a root-level script such as `run-tests.sh` to automate repeated tests and save output files for each algorithm.

Example workflow:

1. Reset metrics
2. Run ApacheBench for a specific algorithm
3. Save `ab` output to a text file
4. Save `/Metrics` output to a JSON file

## Metrics Collected

The LoadManager tracks:

* Total requests
* Successful requests
* Failed requests
* Requests per algorithm
* Per-node total requests
* Per-node failed requests
* Active connections per node

These metrics help compare how each algorithm distributes traffic and handles concurrent load.

## Notes

* `Round Robin` is the simplest and most balanced algorithm when all nodes have similar capacity.
* `Weighted Round Robin` gives more traffic to nodes with higher configured weights.
* `Least Connections` chooses the node with fewer active requests.
* `Hash-Based Selection` maps requests to nodes using a hash-derived index.
* For heavy stress testing, start with `n=1` before increasing the workload parameter.

## Future Improvements

Potential improvements for later versions:

* Read node configuration from `appsettings.json` instead of hardcoding
* Add real health checks between LoadManager and NodeServers
* Add response-time-based balancing
* Add CPU and memory monitoring for containers
* Export benchmark results to CSV or Excel automatically
* Build a dashboard for live visualization

## CV-Friendly Summary

Built a .NET-based load balancing simulation system with a Load Manager and multiple backend Node Servers, implementing and benchmarking Round Robin, Weighted Round Robin, Random, Least Connections, and Hash-based selection under concurrent stress testing using Docker and ApacheBench.

## Author

**Mohamed Saad**

* GitHub: `Mohamed-software-engineer`
* LinkedIn: www.linkedin.com/in/mohamed-saad-engineering
