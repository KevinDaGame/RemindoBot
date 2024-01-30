# RemindoBot

## Running
You will need a mysql database to run remindobot. You can use the following command to start a mysql container using docker:
```docker run --name mysql -e MYSQL_ROOT_PASSWORD=root -p 3306:3306 -d mysql:latest```