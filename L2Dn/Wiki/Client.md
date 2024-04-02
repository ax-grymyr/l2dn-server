# Client

## Steps to prepare the client

### 1. Downloading the client

You can download client from http://akumu.ru website. Any client for 447 protocol should work.

I use [L2EU-P447-D20240313-P-230809-240318-1](http://akumu.ru/lineage2/L2EU/P447/L2EU-P447-D20240313-P-230809-240318-1/) client version for testing the server.

You can download ready-to-run `system` folder instead for this client version [here](https://mega.nz/folder/AS8mBIDC#HchW9hqrcNsQXCdvnAYcZQ).

### 2. Patching client files

Alternatively you patch the client yourself.

Download `patcher.zip` and `Core.dll.zip` [here](https://mega.nz/folder/AS8mBIDC#HchW9hqrcNsQXCdvnAYcZQ) 
and extract archives.

Run `patcher.exe` from the client's `system`, `system\eu` directories, for example:
- open command line (`cmd` or `pwsh`, change current directory to the client `system` directory, for example using command `cd D:\L2\L2EU-P447-D20240313-P-230809-240318-1\system`
- run patcher `<path_to_patcher>\patcher.exe`
- change directory to `system\eu`: `cd eu`
- run patcher `<path_to_patcher>\patcher.exe`
- wait for patcher to finish

Replace `Core.dll` in `system` with the patched one from the `Core.dll.zip` archive.

> [!WARNING]
> The patched `Core.dll` was taken from [forum.ragezone.com](https://forum.ragezone.com/resources/l2j-share-l2jmobius-essence-7-3-sevensigns-1-dec-2023-protocol-447-client-447-source-code.38/). 

### 3. `L2.ini` modification

The next step is the modification of `L2.ini`.

- Decrypt `L2.ini` using the command `l2encdec.exe -d L2.ini L2.ini.txt`.
- Open `L2.ini.txt` in any text editor.
- Find and change the following parameters:
  - `ServerAddr=127.0.0.1`
  - `ExternalLogin=false`
  - `CmdLineLogin=false`
  - `FreeUserTwoClient=false`
  - `FrostModule=false`
  - `UseAutoSoulShotClassic=true`
- Replace `L2.ini` with the modified version using the command `l2encdec.exe -e 413 L2.ini.txt L2.ini`

Now you can run the server locally and connect with this client to the server.

### 4. Replacing Russian item names with English names (optional)

EU client contains Russian item names for Classic edition for some reason (probably left from Russian Classic client).
To replace Russian item names with English names, replace 2 files from the [system.zip](https://mega.nz/folder/AS8mBIDC#HchW9hqrcNsQXCdvnAYcZQ).
These 2 files are `eu\ItemName_Classic-eu.dat` and `eu\L2GameDataName.dat`.
I generated these 2 files for the `L2EU-P447-D20240313-P-230809-240318-1` client version and they may not work for another version.

> [!NOTE]
> The code which replaces Russian item names with English names 
> is currently in `L2Dn/L2Dn.UnitTests/DatReaderTests.cs` in `ReplaceRuItemNamesInEuClient` unit test. 