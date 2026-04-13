#!/bin/bash

# La URI que inyecta Render tiene este formato:
# postgres://USER:PASSWORD@HOST:PORT/DATABASE

# Vamos a extraer cada parte de la URI
uri=$DATABASE_URL
user=$(echo $uri | cut -d '/' -f3 | cut -d ':' -f1)
password=$(echo $uri | cut -d '/' -f3 | cut -d ':' -f2 | cut -d '@' -f1)
host_port=$(echo $uri | cut -d '/' -f3 | cut -d '@' -f2)
host=$(echo $host_port | cut -d ':' -f1)
port=$(echo $host_port | cut -d ':' -f2)
database=$(echo $uri | cut -d '/' -f4)

# Construimos la cadena en formato ADO.NET
export ConnectionStrings__DefaultConnection="Host=$host;Port=$port;Database=$database;Username=$user;Password=$password;SSL Mode=Require;Trust Server Certificate=true;"

# Iniciamos la aplicación .NET
exec dotnet UsersApi.dll