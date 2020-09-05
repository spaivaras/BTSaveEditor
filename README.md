# Barotrauma save editor

This cli app is able to extract submarine files and gamesession from [Barotrauma](https://store.steampowered.com/app/602960/Barotrauma/) save files. Both multiplayer and single player. It also will uncompress submarine file to simple xml for quick edits. After edits this app can then recreate the save from provided session and sub files (both xml or compressed one)

## Usage
**BTSaveEditor [options] save_file.save**

Extract everything from save:

    BTSaveEditor input_save.save

Recreate save file from gamesession.xml and sumbarine.sub.xml

    BTSaveEditor -w --session gamesession.xml --xml submarine.sub.xml output.save

Recreate save file from gamesession.xml and submarine.sub

    BTSaveEditr -w --session -gamesession.xml --sub submarine.sub output.save

### Parameters

 - -w/--write: Write mode writes save instead of reading it
 - -s/--session: Path to session.xml file. Used with -w
 - --sub: Path to submarine.sub file. Used with -w
 - --xml: Path to submarine.sub.xml file. Used with -w (**NOTE!** file name should be .sub.xml)

## Development
Writen using .NET Core 3.1
