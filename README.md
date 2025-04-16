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

12. Implemented Single and Multiple Replica Propagation:

- **If you use a write command in master like Set or Del, it will propagate to the connected Slaves**

13. Implemented Acknowlendgment from master to slave

- **After completion of handshakes, if a client send a `REPLCONF GETACK *` command then master asks for acknowledgement from slaves and slaves replies it with there offset propagated/bytes propagated(Only SET and PING commands are propagated).**

14. Wait command implemented

- **When you run `WAIT <slave_count> <wait_time>` if you have run a write command or ping command before this then the client will wait until <slave_count> number of slaves are processed or the <wait_time> is elapsed whichever satisfies first**

15. Type Command implemented

- **When you want to know the data type of key you can run `TYPE <key>`, if this key exits then the type will be returned else null will be returned(for now only string, integer, boolean types are returned others are returned as string itself)**

16. Implemented XADD command which Creats stream type data and also validating there stream Ids

- **when you want to add stream type data you can run `XADD stream_key stream_id <key1> <value1> <key2> <value2>` to create a stream type data which can have entries and also only the validating/correct stream Ids can be used which means every new streamId should be incremental.**
- **E.g. Stream Ids are of this type 0-1 in which first 0 is in milliseconds and 1 is entry number so every stream Id should be unique even if they are added at same milli second, it should have different entry number.**
- **You can also generate partial stream Ids like `XADD stream_key 12345-* <key1> <value1>` this will generate the sequence number automatically that doesn't exist already in the database for this time stamp part of streamId.**
- **You can also generate Full stream Ids like `XADD stream_key * <key1> <value1>` this will generate the time part as well as sequence number automatically which doesn't exist already in the database.**

17. Implemented XRANGE command which retrieves a range of entries from a stream.

- **when you want to retrieve entries from start specific id to end specific id from a stream key you can use this command `XRANGE stream_key <streamId1> <streamId2>`.**
- **you can also use - in place of streamId1 to get all the ids of that stream till the end stream id like this: `XRANGE stream_key - <streamId2>`.**
- **you can also use + in place of streamId2 to get all the ids of the stream after streamId1 to till last streamId like this: `XRANGE stream_key <streamId1> +`.**

18. Implemented XREAD command which read data from one or more streams, starting from a specified entry ID.

- **when you want to retrieve entries from multiple stream and for specified streamId explicitly, you can use this like: `XREAD streams <stream_key1> <stream_key2> <stream1_Id> <stream2_Id>`.**
- **when you use block arguement with this XREAD command you can make it wait for specific time in which another client on same network can give new data and the the new data will also be displayed, it can be said that it works asynchronously by using block.**
- **E.g. `XREAD block <block_time> streams <stream_key1> <stream_key2> <stream1_Id> <stream2_Id>` after this running this you can give another `XADD <stream_key1>` like command by connecting to another redis-client on same port.**
- **If you want first instance of the redis-cli doesn't time out and responds with null no matter how much time passes. It will wait until another entry is added so we can use `XREAD block 0 streams <stream_key> <stream_Id>`.**
- **If you want to display only the new entry added after giving the command or display (nil) if the block time expires, you can use `XREAD block <block_time> streams <stream_key> $`.**

19. Implemented INCR command which increments the value of a key by 1.

- **It only increments the value if it is in integer otherwise throws a error.**
- **It creates a new key if key doesn't exits and set it as 1.**
- **E.g. `INCR age` if age exists it will increment it by 1.**

20. Implemented MULTI command which enables the transaction state in which every command given by client is queued and only executed when EXEC command is sent.

- **First you need to send `MULTI` command now you are in transaction state, so every command given will be queued and not executed, we can also check if it is executed or not by connecting anotherc client and access the key or something whose related command you queued.**
- **It also gives error for those commands which were incorrect in any way after you give `EXEC` command.**
- **You can also use `DISCARD` to abort a transaction which will discard all the commands queued during this transaction.**
- **It also supports multiple transactions at same time from different clients and also ensures there isn't race conditions occuring between clients commands.**

## 🛠️ Installation

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/en-us/download)

### Steps

1.  Clone the repository:

    ```bash
    git clone https://github.com/Adweeb252/my-own-reddis-Csharp.git
    ```

2.  Navigate to the project directory:

    ```bash
    cd my-own-reddis-Csharp
    ```

3.  Build the project:

    ```bash
    dotnet build
    ```

4.  Run the server:

    ```bash
    dotnet run -- --dir /opt/homebrew/redis --dbfilename dump.rdb
    ```

    - Use `--dir` and `--dbfilename` to specify the path to the RDB file.
    - Example: `dotnet run -- --dir /opt/homebrew/redis --dbfilename dump.rdb`

## ⚙️ Command-Line Options

- `--port`: Specifies the port number for the server. Default is 6380. Example: `dotnet run -- --port 6381`
- `--replicaof`: Configures the server as a slave. Example: `dotnet run -- --port <slave_port> --replicaof "127.0.0.1 <master_port>"`

## Acknowledgements

- **Inspired and guided by the [Codecrafters "Build Your Own Redis" challenge](https://codecrafters.io/challenges/redis). Thanks for the clear stage instructions that helped me structure and deepen my understanding of Redis internals.**
