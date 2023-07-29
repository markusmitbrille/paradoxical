# User Manual

The user manual contains nifty tricks and tips, as well as detailed explanations, and guides on how to use Paradoxical™, the Content Mod Creator.

(Paradoxical™ isn't actually a registered trademark, don't sue me plz.)

## Saving & Loading
The data of your mod project is saved in an SQLite database. When starting up Paradoxical™, an in-memory database is created so you can start modding immediately. This in-memory database, however, is lost upon exiting, or opening another database. Therefore it's important to remember to save it to the disk before doing so, by backing it up first (don't worry, Paradoxical™ will remind you, if you forget).

Project data is saved in binary format to an SQLite database. It can be opened, viewed, and edited via other software, such as [DB Browser for SQLite](https://sqlitebrowser.org/) or other ones like it.

Opening a database establishes a connection to it, through which changes are saved automatically and continuously as you work. Though it isn't necessary, you can manually commit changes you made any time you like.
