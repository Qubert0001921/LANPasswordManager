@echo off

docker compose -f ./docker-compose.development.yml build web-ui
docker compose -f ./docker-compose.development.yml up web-ui -d