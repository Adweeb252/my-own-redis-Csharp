# my-own-reddis-Csharp

**Note:** Multiple clients can connect to this server simultaneously and send multiple commands.

1. **"Ping" Command Implemented** - Returns **PONG**.
2. **"Echo" Command Implemented** - If an argument is provided, the argument is returned; otherwise, an error is returned.
3. **"Set" & "Get" Commands Implemented** - These work as key-value mapping, similar to Redis.
4. Added expiry functionality in the **"Set" Command** using the **px** keyword (in milliseconds).
5. Started RDB Persistence and implemented the **"Config Get" Command** to retrieve the directory and database filename.
6. The server can now read RDB files, sync key-value pairs, and use the **"KEYS" Command** to fetch keys.

   Use `--dir` and `--dbfilename` to specify the path.

   Example: `dotnet run -- --dir /opt/homebrew/redi --dbfilename dump.rdb`

7. The server can now read keys with expiry times in both millisecond and second formats.
8. Using the **"EX" Option** in the **SET Command**, you can set a key with an expiry time in seconds.
9. You can specify a custom port number for the server using the **--port** option.

   Example: `dotnet run -- --port 6381`

   **Note:** If no port is specified, the server will run on the default port **6380**.

10. **"Info Replication" Command Implemented** - Currently returns hardcoded data; however, the role is dynamic depending on the flag provided.

    Use **--replicaof** for slave mode; by default, you will be a master.

11. Implemented handshake functionality with four steps:

    - **PING Command** - Receives `+PONG`.
    - **REPLCONF listening-port** - Receives `+OK`.
    - **REPLCONF capa** - Receives `+OK`.
    - **PSYNC Command** - Receives `+FULLRESYNC <master_repl_id> <master_repl_offset>`.

    (The slave sends these handshakes to the master, and the master responds accordingly.)

### Example for Using Master and Slave:

- In the first terminal, run:  
  `dotnet run -- --port <master_port>`
- In the second terminal, run:  
  `dotnet run -- --port <slave_port> --replicaof "127.0.0.1 <master_port>"`
- Handshakes will happen automatically, and you will receive a final console message:  
  **"Slave is connected to the master."**

12. Implemented Single Replica Propagation:

- **If you use a write command in master like Set or Del, it will propagate to the connected Slave**
