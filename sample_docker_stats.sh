#!/bin/bash

OUTPUT_FILE=$1
INTERVAL=${2:-1}

if [ -z "$OUTPUT_FILE" ]; then
  echo "Usage: ./sample_docker_stats.sh <output_file> [interval_seconds]"
  exit 1
fi

echo "timestamp,container,cpu_percent,mem_usage,mem_percent" > "$OUTPUT_FILE"

while true
do
  TIMESTAMP=$(date +"%Y-%m-%d %H:%M:%S")

  docker stats --no-stream --format "{{.Name}},{{.CPUPerc}},{{.MemUsage}},{{.MemPerc}}" \
  | while IFS= read -r line
    do
      echo "$TIMESTAMP,$line"
    done >> "$OUTPUT_FILE"

  sleep "$INTERVAL"
done