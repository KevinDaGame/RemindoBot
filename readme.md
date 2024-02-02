# RemindoBot

## Running

You will need a mysql database to run remindobot. You can use the following command to start a mysql container using
docker:
```docker run --name mysql -e MYSQL_ROOT_PASSWORD=root -p 3306:3306 -d mysql:latest```

The docker image is published on [docker hub](https://hub.docker.com/repository/docker/kevdadev/remindobot/general)

You need to set the following environment variables:  

| Environment variable    | Value                                          |
|-------------------------|------------------------------------------------|
| DISCORD_TOKEN           | The token of the bot                           |
| DISCORD_GUILD_ID        | The id of the guild where the bot will be used |
| DISCORD_LOG_CHANNEL_ID  | The id of the channel where the bot will log   |
| MYSQL_CONNECTION_STRING | The connection string to the mysql database    |
