using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using CI.QuickSave;
//This script is intended to control all aspects of map creation and management
public class ManageMap : MonoBehaviour
{
    //These four variables are used to define the range of the game map
    public int mapXMax; 
    public int mapXMin;
    public int mapYMax;
    public int mapYMin;
    public int maxEnemies;

    public string saveName;//variable for inputing the save file name

    //This section of the game has 4 overlapping tilemaps. The pointer highlighter is at the very top and will draw over everything else.
    //Under the pointer highlighter is the weapon highlighter which will draw on top of the fog of ware and all other game assets so the player always knows the range of their weapon
    //Under the weapon highlighter is the fog of war that will obscure all planets, ships, and background tiles
    //Finally, at the very bottom is the background tilemap that contains the randomly assigned star tiles which make up the background
    public Tilemap fogOfWar; //Variable to hold a reference to the overlay tiles
    public Tilemap starField; //Variable to hold a reference to the background tiles
    public Tilemap highlightPointerMap;//Variable to hold a reference to the target tile the follows the pointer
    public Tilemap highlightWeaponMap;//Variable to hold a reference to the highlighted laser range tiles


    public GameObject enemyA;
    public GameObject enemyB;
    public GameObject player; //variable for the player game object

    //The following variables set the tiles that are used to set the tile objects for populating the map
    public Tile fogTile;
    public Tile highlightTile;
    public Tile highlightTileRed;
    public Tile starTile0;
    public Tile starTile1;
    public Tile starTile2;
    public Tile starTile3;
    public Tile starTile4;
    public Tile starTile5;
    public Tile starTile6;
    public Tile starTile7;

    public GameObject planet0;
    public GameObject planet1;
    public GameObject planet2;
    public GameObject planet3;
    public GameObject planet4;
    public GameObject planet5;
    public GameObject planet6;
    public GameObject planet7;
    public GameObject planet8;
    public GameObject planet9;
    public GameObject planet10;

    
    public List<Vector3Int> currentHighlightedTiles; //create a list to hold references to the tiles highlighted by the laser range ability. This list is public so that the abiltiy controller script can access this list to check for in range tiles (not currently implemented)
    public Dictionary<string, GameObject> spawnedEnemies;

    private int revealedLength; //variable to store the length of the list of revealed tiles
    private int mapLength; //variable to store the length of the list of map tiles
    private int randTileIndx; //variable to store a random index used to determine which background tile to set a given hex to
    private float dispInterval = 0.1f; //variable to determine how often to update the cell highlighted by the mouse. In this case it is done every 0.1 seconds

    private GridLayout gridLayout; //create a variable to hold an instance of the grid layout

    private List<MapTile> mapTiles; //create a list to hold the reference to the map tiles
    private List<MapTile> revealedTiles; //create a list of hold the reference to the revealed map tiles
    private List<Vector3Int> revealedTilesRaw; //create a list to hold the unedited references of revealed tile coordinates (this list will have duplicates)
    private List<Vector3Int> revealedTilesUnique; //create a list to hold edited references of revealed tile coordinates (this list should not have duplicates)
    
    
    private List<string> starTileStrings; //list of dictionary names for the star tiles used as the map background
    private Dictionary<string, Tile> starTileDict; //a dictionary (key, value list) that holds all of the possible background tiles

    private MovementController playerState; //variable to hold a reference to the movement controller script
    private Vector3 loadedPlayerTile; //variable to hold a reference to the player grid position saved in memory
    private Vector3 highCell; //variable to hold the coordinates of the currently highlighted grid cell
    private Vector3 lastHighCell; //variable to hold the coordinates of the previously highlighted grid cell. 

    void Awake()
    {
        starTileDict = new Dictionary<string, Tile>(); //create a dictionary to hold key value pairs for all the background star tiles
        //These next lines are all adding the background tiles to the dictionary and associating them with a keyword
        starTileDict.Add("starTile0", starTile0);
        starTileDict.Add("starTile1", starTile1);
        starTileDict.Add("starTile2", starTile2);
        starTileDict.Add("starTile3", starTile3);
        starTileDict.Add("starTile4", starTile4);
        starTileDict.Add("starTile5", starTile5);
        starTileDict.Add("starTile6", starTile6);
        starTileDict.Add("starTile7", starTile7);

        starTileStrings = new List<string>(); //create a list to hold all of the star tile keywords
        //These next lines are adding all of the star tile keywords to the list
        starTileStrings.Add("starTile0"); 
        starTileStrings.Add("starTile1");
        starTileStrings.Add("starTile2");
        starTileStrings.Add("starTile3");
        starTileStrings.Add("starTile4");
        starTileStrings.Add("starTile5");
        starTileStrings.Add("starTile6");
        starTileStrings.Add("starTile7");

        spawnedEnemies = new Dictionary<string, GameObject>(); //create a dictionary to hold spawned enemies
        maxEnemies = 2;

        lastHighCell = new Vector3(0, 0, 0); //set the inital value of the last cell highlighted by the mouse pointer

        player = GameObject.FindGameObjectWithTag("Player"); //get a reference to the player game object

        mapTiles = new List<MapTile>(); //create a list to hold the map tiles
        revealedTiles = new List<MapTile>(); //create a list to hold the revealed tiles
        revealedTilesRaw = new List<Vector3Int>(); //create a list to hold the unedited revealed tile coordinates (this list will have duplicates)
        revealedTilesUnique = new List<Vector3Int>(); //create a list to hold the edited revealed tile coordinates (this list should not have duplicates)
        currentHighlightedTiles = new List<Vector3Int>(); //create a list to hold the tiles currently highlighted by the laser range
        playerState = GameObject.Find("Player").GetComponent<MovementController>(); //get a reference to the player game object
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>(); //get a reference to the grid layout 
        GenerateMap(); //call the function to generate the map.
    }
    private void Update()
    {
        dispInterval -= Time.deltaTime; //increment the timer that is used to limit how often the cell highlighted by the cursor is refreshed
        if (dispInterval < 0) //refresh the highlighted cell when the timer reaches 0
        {
            dispInterval = 0.1f; //reset the timer
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //determine where the cursor is currently located
            highCell = highlightPointerMap.WorldToCell(ray.origin); //convert the cursor location into grid coordinates and save it to a variable
            highlightPointerMap.SetTile(new Vector3Int((int)lastHighCell.x, (int)lastHighCell.y, (int)lastHighCell.z), null); //remove the highlighting from the previously highlighted cell
            highlightPointerMap.SetTile(new Vector3Int((int)highCell.x, (int)highCell.y, (int)highCell.z), highlightTile); //add highlighting to the currently selected cell
            lastHighCell = highCell; //update the last highlighted cell to the currently highlighted cell
        }

        if (player.GetComponent<MovementController>().abilityActive) //check if the laser ability has been activated and apply relevant highlighting if it has
        {
            UpdateHighlight(player.GetComponent<AbilityController>().laserRange, player.GetComponent<MovementController>().playerCellPosition, player.GetComponent<MovementController>().abilityActive); //call the function to update the map highlighting
        }
    }

    public Vector3Int evenq2cube(Vector3Int evenqCoords)
    {
        //this function converts from rectangule coordinates to cude coordinates. This helps with calculating distance 
        //this method of hex coordinates was taken from here https://www.redblobgames.com/grids/hexagons/
        Vector3Int cubeCoords;
        int x = evenqCoords.y;
        int z = evenqCoords.x - ((evenqCoords.y - (Mathf.Abs(evenqCoords.y) % 2)) / 2);
        int y = -x - z;
        cubeCoords = new Vector3Int(x, y, z);
        return cubeCoords;
    }

    public int HexCellDistance(Vector3Int hex1, Vector3Int hex2)
    {
        //this function calculates distance using the cube coordinate method
        //this method of calculating distance was taken from here https://www.redblobgames.com/grids/hexagons/
        return Mathf.Max(Mathf.Abs(hex2.x - hex1.x), Mathf.Abs(hex2.y - hex1.y), Mathf.Abs(hex2.z - hex1.z));
    }

    public void GenerateMap() //this function runs as soon as the scene loads. It's purpose is to either load a map from memory or create a new map for the game to use
    {
        if (QuickSaveRoot.Exists(saveName)) //use the quicksave feature to check if a save file exists 
        {
            Load(); //if a save file exists, call the load function
        }else
        {
            Debug.ClearDeveloperConsole(); //this command is intended to clear the developer log. I do not think it works
            
            for (int x = mapXMin; x <= mapXMax; x++) //iterate through x coordinates to create map
            {
                for (int y = mapYMin; y <= mapYMax; y++) //iterate through y coordinates to create map
                {
                    randTileIndx = Random.Range(0, 8); //generate a random number to use for choosing the background tile
                    mapTiles.Add(new MapTile(x, y, starTileStrings[randTileIndx])); //use the random number to choose a map tile then add it to the list at the current coordinates
                    fogOfWar.SetTile(new Vector3Int(x, y, 0), fogTile); //set the fog of war active at the current coordinates 
                    starField.SetTile(new Vector3Int(x, y, 0), starTileDict[starTileStrings[randTileIndx]]); //set the background hex at the current coordinates based on the random map tile selected before
                }
            }
        }
    }
    public void UpdateFogOfWar(int vision, Vector3Int playerCellPosition) //this script will clear the fog of war based on the player's vision
    {
        Debug.ClearDeveloperConsole(); //this command is intended to clear the developer log. I do not think it works

        //clear surrounding tiles based on what the player can see by looping through tile coordinates around the player
        for (int x = -vision; x <= vision; x++)//start from an x value of negative vision distance and iterate to an x value of positive vision
        {
            for (int y = -vision; y <= vision; y++)//start from an y value of negative vision distance and iterate to an y value of positive vision
            {
                //float fogCellDistance = Vector3.Distance(gridLayout.CellToWorld((playerCellPosition + new Vector3Int(x, y, 0))), gridLayout.CellToWorld(playerCellPosition)); //calculate the distance between the player and the targeted cell
                float fogCellDistance = HexCellDistance(evenq2cube((playerCellPosition + new Vector3Int(x, y, 0))), evenq2cube(playerCellPosition)); //calculate the distance between the player and the targeted cell (convert distances to cube coordinates to get more accurate distance measurements)

                //if (fogCellDistance > 0.33 * vision) //each cell is 32 pixels wide so check that the distance between that player and the given cell is less than that multiplied by the range of the ships sensors
                if (fogCellDistance > vision) 
                {
                    //if the distance is greater than this, then do nothing
                }
                else
                {
                    fogOfWar.SetTile(playerCellPosition + new Vector3Int(x, y, 0), null); //if the range is less than that then remove one of the fog tiles
                    revealedTilesRaw.Add(playerCellPosition + new Vector3Int(x,y,0)); //record the tiles that meet this critera (i.e. the tiles removed). Because tiles that have already been removed will still arrive at this branch of the if statement, this list will need to be pruned to remove duplicates later.
                }
            }
        }
        revealedTilesUnique = new List<Vector3Int>(new HashSet<Vector3Int>(revealedTilesRaw)); //this operation should remove duplicates from the raw tiles list and only keep unique values. This keeps the list from getting unmanagably long. (note that I do not fully understand why this step works. Requires investigation)
    }

    public void UpdateHighlight(int range, Vector3Int playerCellPosition, bool laserState)
    {

        //this operation will function almost identically to clearing the fog of war, except that it will highlight the cells within range of the laser. 
        for (int x = -range; x <= range; x++) //iterate through the range of the laser to generate the x coordinates
        {
            for (int y = -range; y <= range; y++) //iterate through the range of the laser to generate the y coordindates
            {
                //float highlightCellDistance = Vector3.Distance(gridLayout.CellToWorld((playerCellPosition + new Vector3Int(x, y, 0))), gridLayout.CellToWorld(playerCellPosition)); //calculate the distance from the player to the current set of coordinates
                float highlightCellDistance = HexCellDistance(evenq2cube(playerCellPosition + new Vector3Int(x, y, 0)), evenq2cube(playerCellPosition)); //calculate the distance from the player to the current set of coordinates
                
                //if (highlightCellDistance > 0.32 * range) //if the calculated distance is larger than the width of a hex cell times the range of the laser, then do nothing
                if (highlightCellDistance > range) //if the calculated distance is larger than the width of a hex cell times the range of the laser, then do nothing
                {
                    //Do nothing
                }
                else //if the calculated distance is less than the width of a hex cell times the range of the laser then set the proper highlighting on that cell
                {
                    if (laserState) //if the laser is active, then highlight the cells
                    {
                        highlightWeaponMap.SetTile(playerCellPosition + new Vector3Int(x, y, 0), highlightTileRed); //change the cell at the current coordinates to the highlighted tile
                        currentHighlightedTiles.Add(playerCellPosition + new Vector3Int(x, y, 0)); //add the coordinates of this tile to the list of tiles currently highlighted

                    }
                    else //if the laser is not active, then disable the highlighted cells
                    {
                        highlightWeaponMap.SetTile(playerCellPosition + new Vector3Int(x, y, 0), null); //set the cell at the current coordinates to null
                        currentHighlightedTiles.Clear(); //clear the list of highlighted cell coordinates. This does not need to be done every single time. Maybe move this out of the loop to save processing power?
                    }
                }
            }
        }        
    }
    public void ResetMap() //this function exists specifically to return the map to a hidden state. Only for debugging at the moment, but maybe there is a mechanical use for this
    {

        for (int x = mapXMin; x <= mapXMax; x++) //iterate though the x boundaries of the map
        {
            for (int y = mapYMin; y <= mapYMax; y++) //iterate though the y boundaries of the map
            {
                fogOfWar.SetTile( new Vector3Int(x, y, 0), fogTile); //set the tile at the given coordinates to the fog tile
            }
        }
    }
    public void Save() //this is the first part of three operations used to manage the game save file. 
    {
        //the purpose of this script is to format and write the save file for the game
        QuickSaveWriter instWriter = QuickSaveWriter.Create(saveName); //create an instance of the QuickSaveWriter
        int i = 0; //create an index tracker so that each tile can have a unique name
        foreach (Vector3Int tile in revealedTilesUnique) //iterate through each tile in the unique revealed tiles list (which is only coordinates) and then create and add a map tile object to the revealed tiles map list. This appears to be a redundant operation since this list is not used and a coordinates list is simply recreated a couple steps later. Consider removing
        {
            revealedTiles.Add(new MapTile(tile.x, tile.y,null)); //create map tile objects for all the revealed mip tile coordinates and save them to a list
        }
        revealedLength = revealedTiles.Count; //determine how long the revealed map tiles list is and save that to a varilable 
        mapLength = mapTiles.Count; //determine how long the map tiles list is and save that to a variable 
        instWriter.Write<int>("revealedLength", revealedLength); //write the revealed tiles length to the save file to be used when loading
        instWriter.Write<int>("mapLength", mapLength); //write the map tiles length to the save file to be used when loading
        instWriter.Write<Vector3>("playerPos",playerState.playerCellPosition); //write the player position to the save file
        foreach (MapTile revealedTile in revealedTiles)//loop through the revealed tiles list and save off each to the save file
        {
            Vector2 saveItem = new Vector2(revealedTile.xCoordinate, revealedTile.yCoordinate); //create a vector two that can be saved to the file because map tiles can't be saved (wrong. Map tiles can be saved. Must have written this before I figured out how)
            instWriter.Write<Vector2>("revealedTile" + i.ToString(), saveItem); //write the coordinates to the save file along with a keyword to identify them
            i++; //increment the index tracker
        }
        i = 0; //after the loop finishes, reset the index tracker to 0
        foreach (MapTile mapTile in mapTiles)//loop through the map tiles list and save off each to the save file
        {
            Vector2 saveItem = new Vector2(mapTile.xCoordinate, mapTile.yCoordinate);//this step might be redundant because this operation is now saving map tile directly to the save file
            instWriter.Write<MapTile>("mapTile" + i.ToString(), mapTile);//write the map tile directly to the save file
            i++;//increment the index tracker
        }
        instWriter.Commit();//write the save file
        print("Saved");//send a message that the file has been saved
    }

    public void Load() //This is the second of three operations used to manage the game save file. 
    {
        if (QuickSaveRoot.Exists(saveName)) //if a save file exists, load data from that file
        {
            QuickSaveReader instReader = QuickSaveReader.Create(saveName); //create an instance of the quick save reader to pull in the save file            
            revealedLength = instReader.Read<int>("revealedLength"); //extract the revealed tiles list length parameter from the save file
            mapLength = instReader.Read<int>("mapLength"); //extract the map tiles list length parameter from the save file
            loadedPlayerTile = instReader.Read<Vector3>("playerPos"); //extract the player position coordinates from the save file
            playerState.transform.position = gridLayout.CellToWorld(new Vector3Int ((int)loadedPlayerTile.x, (int)loadedPlayerTile.y, (int)loadedPlayerTile.z)); //set the position of the player basded on the data extracted from the save file
            for(int i = 0; i < mapLength; i++) //set up a for loop to iterate through the save file and extract all of the map tiles. This uses the map tiles length to shorten the number of iterations for this loop
            {
                MapTile mapItem = instReader.Read<MapTile>("mapTile" + i.ToString()); //create map tiles based on data extracted from save file 
                mapTiles.Add(mapItem); //create a list of map tiles extracted from the save file
                starField.SetTile(new Vector3Int((int)mapItem.xCoordinate, (int)mapItem.yCoordinate, 0), starTileDict[mapItem.tileString]); //set the star tiles based on the data extracted from the save file
                fogOfWar.SetTile(new Vector3Int((int)mapItem.xCoordinate, (int)mapItem.yCoordinate, 0), fogTile); //set the fog of war based on the data extracted from the save file
            }            
            for (int i=0; i < revealedLength;i++) //loop through the save file and extract the data on the revealed tiles
            {
                Vector2 loadedItem = instReader.Read<Vector2>("revealedTile" + i.ToString()); //extract the vector2 data from the save file for the revealed tiles
                fogOfWar.SetTile(new Vector3Int((int)loadedItem.x, (int)loadedItem.y, 0), null); //set the fog of war based on the revealed tiles
                revealedTilesRaw.Add(new Vector3Int((int)loadedItem.x, (int)loadedItem.y, 0)); //save the coordinates from this to the revealed tiles raw list. This is important because this list needs to be checked against the tiles that get checked each time to ensure that only unique tiles go into the final list.
            }
            print("Loaded"); //send a message that data was loaded
        }
        else //Save file does not exist, load all as defaults
        {
            GenerateMap(); //if not save file was found, simply create a new map
            print("No save data found, building new map"); //send a message that no save data was found
        }
    }

    public void Delete() //This is the final of three operations used to manage the game save file. 
    {
        if (QuickSaveRoot.Exists(saveName)) //check if the file exists
        {
            QuickSaveRoot.Delete(saveName); //if the file exists, then delete it
            print("Deleted data file " + saveName); //send a message that the file was deleted
        }
        else
        {
            print("Nothing to delete"); //if no save file exists, send a message that nothing was done
        }
    }

    public void SpawnPlanets(int spawnMaxX, int spawnMaxY, int spawnMinX, int spawnMinY, bool repeatPlanets, List<int> allowedPlanets, int maxPlanets)
    {
        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");
        List<GameObject> availablePlaents = new List<GameObject>() { planet0, planet1, planet2, planet3, planet4, planet5, planet6, planet7, planet8, planet9, planet10 };
        List<GameObject> allowablePlanets = new List<GameObject>();
        List<Vector3Int> availableSpawnPoints = new List<Vector3Int>();
        
        foreach (GameObject planet in planets)
        {
            GameObject.Destroy(planet);
        }

        foreach(int planet in allowedPlanets)
        {
            allowablePlanets.Add(availablePlaents[planet]);
        }

        for (int x = spawnMinX; x <= spawnMaxX; x++) //iterate through x coordinates to create map
        {
            for (int y = spawnMinY; y <= spawnMaxY; y++) //iterate through y coordinates to create map
            {
                availableSpawnPoints.Add(new Vector3Int(x, y, 0));
            }
        }

        int maxNoRepeatPlanets = allowablePlanets.Count;

        for (int planetsSpawned = 0; planetsSpawned <= maxPlanets; planetsSpawned++)
        {
            if (!repeatPlanets && planetsSpawned >= maxNoRepeatPlanets )
            {
                break;
            }
            else
            {
                int randPlanetIndex = Random.Range(0, allowablePlanets.Count);
                int randSpawnIndex = Random.Range(0, availableSpawnPoints.Count);

                if (repeatPlanets)
                {
                    Instantiate(allowablePlanets[randPlanetIndex], starField.CellToWorld(availableSpawnPoints[randSpawnIndex]), Quaternion.identity);
                    availableSpawnPoints.RemoveAt(randSpawnIndex);
                }
                else
                {
                    Instantiate(allowablePlanets[randPlanetIndex], starField.CellToWorld(availableSpawnPoints[randSpawnIndex]), Quaternion.identity);
                    availableSpawnPoints.RemoveAt(randSpawnIndex);
                    allowablePlanets.RemoveAt(randPlanetIndex);
                }
            }
        }
    }

    public void SpawnEnemies(int maxEnemies, bool repeatEnemies, List<int> allowedEnemies)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<GameObject> availableEnemies = new List<GameObject>() { enemyA, enemyB };
        List<GameObject> allowableEnemies = new List<GameObject>();
        List<Vector3Int> availableSpawnPoints = new List<Vector3Int>();

        foreach (GameObject enemy in enemies)
        {
            GameObject.Destroy(enemy);
        }

        foreach (int enemy in allowedEnemies)
        {
            allowableEnemies.Add(availableEnemies[enemy]);
        }

        availableSpawnPoints.AddRange(revealedTilesUnique);

        int maxNoRepeatEnemies = allowableEnemies.Count;

        for (int enemiesSpawned = 0; enemiesSpawned <= maxEnemies; enemiesSpawned++)
        {
            if (!repeatEnemies && enemiesSpawned >= maxNoRepeatEnemies)
            {
                break;
            }
            else
            {
                int randEnemyIndex = Random.Range(0, allowableEnemies.Count);
                int randSpawnIndex = Random.Range(0, availableSpawnPoints.Count);

                if (repeatEnemies)
                {
                    Instantiate(allowableEnemies[randEnemyIndex], starField.CellToWorld(availableSpawnPoints[randSpawnIndex]), Quaternion.identity);
                    availableSpawnPoints.RemoveAt(randSpawnIndex);
                }
                else
                {
                    Instantiate(allowableEnemies[randEnemyIndex], starField.CellToWorld(availableSpawnPoints[randSpawnIndex]), Quaternion.identity);
                    availableSpawnPoints.RemoveAt(randSpawnIndex);
                    allowableEnemies.RemoveAt(randEnemyIndex);
                }
            }
        }
    }

    public void GenericSpawnPlanets()
    {
        List<int> allowedPlaents = new List<int>() {0,1,2,3,4,5,6,7,8,9,10};
        //List<int> allowedPlaents = new List<int>() {5,8};
        SpawnPlanets(mapXMax - 10, mapYMax - 10, mapXMin + 10, mapYMin + 10, true, allowedPlaents, 20);
    }
    public void GenericSpawnEnemies()
    {
        List<int> allowedEnemies = new List<int>() { 0, 1};
        SpawnEnemies(5, true, allowedEnemies);
    }
}
