@echo off

docker compose -f ./docker-compose.development.yml down
docker compose -f ./docker-compose.development.yml up -d