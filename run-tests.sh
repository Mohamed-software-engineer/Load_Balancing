#!/bin/bash

set -e

BASE_URL="http://localhost:8090/LoadBalancer/cal?n=1"
METRICS_URL="http://localhost:8090/Metrics"
RESET_URL="http://localhost:8090/Metrics/reset"
NODES_URL="http://localhost:8090/Nodes"

REQUESTS=10000
CONCURRENCY=200
ALGORITHMS=("rr" "random" "wrr" "lc" "hash")

mkdir -p results

echo "Starting Docker containers..."
docker-compose up --build -d

echo "Waiting for services to be ready..."
sleep 8

echo "Checking services..."
curl -s "$NODES_URL" > /dev/null

for algo in "${ALGORITHMS[@]}"
do
  echo ""
  echo "========================================"
  echo "Running benchmark for: $algo"
  echo "========================================"

  curl -s -X POST "$RESET_URL" > /dev/null

  CPU_FILE="results/${algo}-cpu.csv"
  AB_FILE="results/${algo}-ab.txt"
  METRICS_FILE="results/${algo}-metrics.json"

  echo "Starting Docker CPU sampler..."
  ./sample_docker_stats.sh "$CPU_FILE" 1 &
  SAMPLER_PID=$!

  sleep 1

  echo "Running ApacheBench..."
  ab -n $REQUESTS -c $CONCURRENCY "${BASE_URL}&algo=${algo}" > "$AB_FILE"

  echo "Stopping Docker CPU sampler..."
  kill $SAMPLER_PID || true
  wait $SAMPLER_PID 2>/dev/null || true

  curl -s "$METRICS_URL" > "$METRICS_FILE"

  echo "Saved:"
  echo "  $AB_FILE"
  echo "  $CPU_FILE"
  echo "  $METRICS_FILE"
done

echo ""
echo "Generating comparison report..."
python3 compare_results.py

echo ""
echo "Done."
echo "Check:"
echo "  results/summary.txt"
echo "  results/summary.csv"