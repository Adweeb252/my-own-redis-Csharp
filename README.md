# my-own-reddis-Csharp

Note:- Multiple clients can connect with this server simultaneously and can send multiple commands.

1. "Ping" Command Implemented - It returns PONG
2. "Echo" command Implemented - If arguement is given with this, the arguement is returned, if not an error is returned.
3. "Set" & "Get" command Implemented - it works as mapping just like in redis.
4. Added Expiry functionality in Set command with px keyword (in milliseconds).
5. Started RDB Persistence and Implemented "Config Get command" to get the dir and dbfilename.
6. Now server can read rdb files and sync key-value pairs and also use "KEYS" command to fetch keys.
   (Use --dir and --dbfilename for specifying the path
   E.g:- dotnet run -- --dir /opt/homebrew/redi --dbfilename dump.rdb)
7. Now server can also read keys with expiry time in it in both millisecond and seconds format.
8. Now using "EX" option in SET command we can set key with expiry time in seconds.
9. Now you can specify custom port number for the server to run using --port
   (E.g:- dotnet run -- --port 6381). Note: If port not specified it will run on default 6380 port.
10. "Info replication" command implemented but it is giving hard coded data for now (only roles is dynamic depending on flag you give.
    --replicaof is for slave by default you will become master)
11. Implemented handshake functionality in which we are sending 4 handshakes:-
    a. PING command - recieves +PONG
    b. REPLCONF listening-port - recieves +OK
    c. REPLCONF copa - recieves +OK
    d. PSYNC - recieves +FULLRESYNC <master_repl_id> <master_repl_offset>
    (Slave sends these handshakes to the master and master sends reponse to it)
    E.g for using master and slave:-
    a. In first terminal run "dotnet run -- --port <master_port>
    b. In second terminal run "dotnet run -- --port <slave_port> --replicaof "127.0.0.1 <master_port>"
    c. Now the handshakes will happen automatically and you will recieve a final console message as "Slave is connected to the master"
