using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using CI.QuickSave;
using System.IO;
using UnityEditor;
//This script is intended to control all aspects of map creation and management
public class ManageMap : MonoBehaviour
{
    //These four variables are used to define the range of the game map
    public int mapXMax; 
    public int mapXMin;
    public int mapYMax;
    public int mapYMin;
    public int maxEnemies;
    public int enemiesInList;

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
    public GameObject starGate;

    
    public List<Vector3Int> currentHighlightedTiles; //create a list to hold references to the tiles highlighted by the laser range ability. This list is public so that the abiltiy controller script can access this list to check for in range tiles (not currently implemented)


    private int revealedLength; //variable to store the length of the list of revealed tiles
    private int mapLength; //variable to store the length of the list of map tiles
    private int randTileIndx; //variable to store a random index used to determine which background tile to set a given hex to
    //private int starGateSpawnPoint;
    private float dispInterval = 0.1f; //variable to determine how often to update the cell highlighted by the mouse. In this case it is done every 0.1 seconds
    private Vector3Int starGateSpawnPoint;

    private GridLayout gridLayout; //create a variable to hold an instance of the grid layout
    private GameObject gameController;

    private List<MapTile> mapTiles; //create a list to hold the reference to the map tiles
    private List<MapTile> revealedTiles; //create a list of hold the reference to the revealed map tiles
    private List<Vector3Int> revealedTilesRaw; //create a list to hold the unedited references of revealed tile coordinates (this list will have duplicates)
    public List<Vector3Int> revealedTilesUnique; //create a list to hold edited references of revealed tile coordinates (this list should not have duplicates)
    public List<PlanetObject> spawnedPlanets;
    public List<EnemyObject> spawnedEnemies;

    private List<string> starTileStrings; //list of dictionary names for the star tiles used as the map background
    private Dictionary<string, Tile> starTileDict; //a dictionary (key, value list) that holds all of the possible background tiles
    private Dictionary<string, GameObject> planetObjectsDict;
    private Dictionary<string, GameObject> enemyObjectsDict;

    private MovementController playerState; //variable to hold a reference to the movement controller script
    private Vector3 loadedPlayerTile; //variable to hold a reference to the player grid position saved in memory
    private Vector3 highCell; //variable to hold the coordinates of the currently highlighted grid cell
    private Vector3 lastHighCell; //variable to hold the coordinates of the previously highlighted grid cell. 
    private AbilityController abilityController;
    private UIControl uiController;
    private ResourceAndUpgradeManager resourceAndUpgradeManager;
    private MovementController movementController;

    void Awake()
    {
        //The following section deals with creating and moving the tutorial level save file so that it is always consistent
        //Debug.Log(Path.Combine(Application.persistentDataPath, "QuickSave"));
        string quicksavePath = Path.Combine(Application.persistentDataPath, "QuickSave");
        string quicksaveFilePath = Path.Combine(quicksavePath, "TutorialFile.json");
        //Debug.Log(Application.dataPath);
        string dataPath = Application.dataPath;
        string dataPathTutorial = Path.Combine(dataPath, "TutorialFile.json");
        if (Directory.Exists(quicksavePath))
        {
            if (File.Exists(quicksaveFilePath))
            {
                //Debug.Log("I found the file!");
            }
            else
            {
                //Debug.Log("Need to move File");
                //FileUtil.CopyFileOrDirectory(dataPathTutorial, quicksaveFilePath);
                File.Copy(dataPathTutorial, quicksaveFilePath);
            }
            
            
        }
        else
        {
            //Debug.Log("Need to make directory then move file");
            Directory.CreateDirectory(quicksavePath);
            //FileUtil.CopyFileOrDirectory(dataPathTutorial, quicksaveFilePath);
            File.Copy(dataPathTutorial, quicksaveFilePath);
        }
        
        //create a dictionary to hold key value pairs for all the background star tiles
        starTileDict = new Dictionary<string, Tile>() {
            { "starTile0", starTile0 },
            { "starTile1", starTile1 },
            { "starTile2", starTile2 },
            { "starTile3", starTile3 },
            { "starTile4", starTile4 },
            { "starTile5", starTile5 },
            { "starTile6", starTile6 },
            { "starTile7", starTile7 },
        }; 
        
        //create a list to hold all of the star tile keywords
        starTileStrings = new List<string>() { "starTile0", "starTile1", "starTile2", "starTile3", "starTile4", "starTile5", "starTile6", "starTile7", };

        planetObjectsDict = new Dictionary<string, GameObject>()
        {
            { "Planet0", planet0 },
            { "Planet1", planet1 },
            { "Planet2", planet2 },
            { "Planet3", planet3 },
            { "Planet4", planet4 },
            { "Planet5", planet5 },
            { "Planet6", planet6 },
            { "Planet7", planet7 },
            { "Planet8", planet8 },
            { "Planet9", planet9 },
            { "Planet10", planet10 },
        };
        spawnedPlanets = new List<PlanetObject>();

        enemyObjectsDict = new Dictionary<string, GameObject>()
        {
            { "EnemyA", enemyA },
            { "EnemyB", enemyB },
        };
        spawnedEnemies = new List<EnemyObject>();

        lastHighCell = new Vector3(0, 0, 0); //set the inital value of the last cell highlighted by the mouse pointer

        player = GameObject.FindGameObjectWithTag("Player"); //get a reference to the player game object
        gameController = GameObject.Find("GameController");
        abilityController = player.GetComponent<AbilityController>();
        mapTiles = new List<MapTile>(); //create a list to hold the map tiles
        revealedTiles = new List<MapTile>(); //create a list to hold the revealed tiles
        revealedTilesRaw = new List<Vector3Int>(); //create a list to hold the unedited revealed tile coordinates (this list will have duplicates)
        revealedTilesUnique = new List<Vector3Int>(); //create a list to hold the edited revealed tile coordinates (this list should not have duplicates)
        currentHighlightedTiles = new List<Vector3Int>(); //create a list to hold the tiles currently highlighted by the laser range
        playerState = GameObject.Find("Player").GetComponent<MovementController>(); //get a reference to the player game object
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>(); //get a reference to the grid layout 
        uiController = gameController.GetComponent<UIControl>();
        resourceAndUpgradeManager = gameController.GetComponent<ResourceAndUpgradeManager>();
        movementController = player.GetComponent<MovementController>();
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

        enemiesInList = spawnedEnemies.Count;
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
            ContextualSpawnPlanets();
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
                        //Debug.Log("Enabling Highlighting"+ playerCellPosition + new Vector3Int(x, y, 0));
                    }
                    else //if the laser is not active, then disable the highlighted cells
                    {
                        highlightWeaponMap.SetTile(playerCellPosition + new Vector3Int(x, y, 0), null); //set the cell at the current coordinates to null
                        currentHighlightedTiles.Clear(); //clear the list of highlighted cell coordinates. This does not need to be done every single time. Maybe move this out of the loop to save processing power?
                        //Debug.Log("disabling highlighting");
                    }
                }
            }
        }        
    }

    public void HighlightSet(List<Vector3Int> highlightSet, bool setState)
    {
        foreach(Vector3Int highlightCell in highlightSet)
        {
            if (setState)
            {
                highlightWeaponMap.SetTile(highlightCell, highlightTileRed);
            }
            else
            {
                highlightWeaponMap.SetTile(highlightCell, null);
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
        instWriter.Write<int>("planetLength", spawnedPlanets.Count); 
        instWriter.Write<int>("enemyLength", spawnedEnemies.Count); 
        instWriter.Write<Vector3>("playerPos",playerState.playerCellPosition); //write the player position to the save file
        instWriter.Write<Vector3Int>("starGateSpawnPoint", starGateSpawnPoint);
        foreach (MapTile revealedTile in revealedTiles)//loop through the revealed tiles list and save off each to the save file
        {
            Vector2 saveItem = new Vector2(revealedTile.xCoordinate, revealedTile.yCoordinate); //create a vector two that can be saved to the file because map tiles can't be saved (wrong. Map tiles can be saved. Must have written this before I figured out how)
            instWriter.Write<Vector2>("revealedTile" + i.ToString(), saveItem); //write the coordinates to the save file along with a keyword to identify them
            i++; //increment the index tracker
        }
        i = 0; //after the loop finishes, reset the index tracker to 0
        foreach (MapTile mapTile in mapTiles)//loop through the map tiles list and save off each to the save file
        {
            instWriter.Write<MapTile>("mapTile" + i.ToString(), mapTile);//write the map tile directly to the save file
            i++;//increment the index tracker
        }
        i = 0; //after the loop finishes, reset the index tracker to 0

        foreach (PlanetObject planetSpawn in spawnedPlanets)
        {
            instWriter.Write<PlanetObject>("spawnedPlanet" + i.ToString(), planetSpawn);
            i++;//increment the index tracker
        }
        i = 0; //after the loop finishes, reset the index tracker to 0
        foreach (EnemyObject enemySpawn in spawnedEnemies)
        {
            instWriter.Write<EnemyObject>("spawnedEnemy" + i.ToString(), enemySpawn);
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
            int planetLength = instReader.Read<int>("planetLength");
            int enemyLength = instReader.Read<int>("enemyLength");
            loadedPlayerTile = instReader.Read<Vector3>("playerPos"); //extract the player position coordinates from the save file
            playerState.transform.position = gridLayout.CellToWorld(new Vector3Int ((int)loadedPlayerTile.x, (int)loadedPlayerTile.y, (int)loadedPlayerTile.z)); //set the position of the player basded on the data extracted from the save file
            starGateSpawnPoint = instReader.Read<Vector3Int>("starGateSpawnPoint");
            Instantiate(starGate, starField.CellToWorld(starGateSpawnPoint), Quaternion.identity);
            for (int i = 0; i < mapLength; i++) //set up a for loop to iterate through the save file and extract all of the map tiles. This uses the map tiles length to shorten the number of iterations for this loop
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
            for(int i=0; i < planetLength; i++)
            {
                PlanetObject planetObject = instReader.Read<PlanetObject>("spawnedPlanet" + i.ToString());
                spawnedPlanets.Add(planetObject);
                Vector3Int planetSpawnPoint = new Vector3Int(planetObject.xCoordinate, planetObject.yCoordinate, 0);
                Instantiate(planetObjectsDict[planetObject.planetString],starField.CellToWorld(planetSpawnPoint),Quaternion.identity);
            }
            GameObject tempEnemy;
            for (int i = 0; i < enemyLength; i++)
            {
                EnemyObject enemyObject = instReader.Read<EnemyObject>("spawnedEnemy" + i.ToString());
                spawnedEnemies.Add(enemyObject);
                Vector3Int enemySpawnPoint = new Vector3Int(enemyObject.xCoordinate, enemyObject.yCoordinate, 0);
                tempEnemy = Instantiate(enemyObjectsDict[enemyObject.enemyString], starField.CellToWorld(enemySpawnPoint), Quaternion.identity);
                string clone = "(Clone)";
                string enemyName = tempEnemy.name;
                Vector3Int enemyPosition = starField.WorldToCell(tempEnemy.transform.position);
                enemyName = enemyName.Replace(clone, "");
                EnemyObject enemyObjectRef = new EnemyObject(enemyPosition.x, enemyPosition.y, enemyName);
                tempEnemy.gameObject.GetComponent<EnemyShipControl>().thisEnemyObject = enemyObjectRef;
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
        /*
         * This script is designed to spawn planets randomly throughout the map. It takes the following inputs
         * spawnMaxX, spawnMaxY, spawnMinX, spawnMinY: These four inputs define the range of coordinates that the planets can spawn within.
         * repeatPlanets: This Boolean defines whether a planet is allowed to repeat or not
         * allowedPlanets: This is a list of the planets that the function can choose from when spawning the planets
         * maxPlanets: This is a parameter that defines how many planets will be spawned (Note: this parameter will be overruled in the case that the available spaces to spawn planets is less than this number)
        */
        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet"); //create a list of any planets in the current scene
        GameObject[] starGates = GameObject.FindGameObjectsWithTag("StarGate");
        List<GameObject> availablePlaents = new List<GameObject>() { planet0, planet1, planet2, planet3, planet4, planet5, planet6, planet7, planet8, planet9, planet10 }; //create a list of planets that can be used based on supplied prefabs
        List<GameObject> allowablePlanets = new List<GameObject>(); //create a placeholder for a list that will define what planets the function is allowed to spawn
        List<Vector3Int> availableSpawnPoints = new List<Vector3Int>(); //create a placeholder for a list that will define where planets are allowed to spawn
        
        foreach (GameObject planet in planets) //loop through and destroy any planets currently in the scene
        {
            GameObject.Destroy(planet);
        }

        foreach(GameObject starGate in starGates)
        {
            Destroy(starGate);
        }
 
        spawnedPlanets.Clear();

        foreach(int planet in allowedPlanets) //loop through the supplied list of allowed planets and build the list of planets that the function is allowed to spawn
        {
            allowablePlanets.Add(availablePlaents[planet]);
        }

        for (int x = spawnMinX; x <= spawnMaxX; x++) //This loop, and the one nested within it, are designed to populate the list of spawn points that can be used by the function
        {
            for (int y = spawnMinY; y <= spawnMaxY; y++)
            {
                availableSpawnPoints.Add(new Vector3Int(x, y, 0));
            }
        }

        availableSpawnPoints.Remove(playerState.playerCellPosition); //remove the current player position from the list of available planets spawn points so that a planet does not spawn under the player

        int maxNoRepeatPlanets = allowablePlanets.Count; //set a value for the maximum planets that can be spawned if the planets are not allowed to repeat. This will simply be equal to the number of available planets provided, but it needs to be saved off here becuase the list of avialble planets is shortened as they spawn for the case where no repeats are allowed.
        int maxSpawnPointsAvailable = availableSpawnPoints.Count; //set a value for the maximum planets that can be spawned based on how many spawn points exist. This functions similarly to the above

        for (int planetsSpawned = 0; planetsSpawned <= maxPlanets; planetsSpawned++) //set up a for loop to spawn planets until the maximum is reached
        {
            if ((!repeatPlanets && planetsSpawned >= maxNoRepeatPlanets)||(planetsSpawned >= maxSpawnPointsAvailable)) //set up an alternate termination conditions for non repeating planets or maximum spawn point exceedance (i.e. if the number of planets spawned is greater than or equal to the number of planets in the list of available planets or if all of the avilable spawn points have been filled then no more planets can be spawned without either repeating or stacking planets respectively and the loop must end)
            {
                break;
            }
            else
            {
                int randPlanetIndex = Random.Range(0, allowablePlanets.Count); //on each loop, generate a random index value between 0 and the length of the allowed planets list. This will determine what planet is spawned in this iteration of the loop
                int randSpawnIndex = Random.Range(0, availableSpawnPoints.Count); //on each loop, generate a random index value between 0 and the length of the available spawn points list. This will determine where the planet is spawned on the map

                if (repeatPlanets) //If planets are allowed to repeat, instantiate a randomly selected planet at the randomly selected coordinates from above, then remove the coordinates from the list of available coordinates
                {
                    GameObject planetTemp;
                    planetTemp = Instantiate(allowablePlanets[randPlanetIndex], starField.CellToWorld(availableSpawnPoints[randSpawnIndex]), Quaternion.identity);
                    availableSpawnPoints.RemoveAt(randSpawnIndex);
                    
                    string clone = "(Clone)";
                    string planetName = planetTemp.name;
                    Vector3Int planetPosition = starField.WorldToCell(planetTemp.transform.position);
                    planetName = planetName.Replace(clone, "");
                    if (planetName == "Planet10")
                    {
                        availablePlaents.RemoveAt(randPlanetIndex); //Only ever allow 1 platinum planet to spawn
                    }
                    spawnedPlanets.Add(new PlanetObject(planetPosition.x, planetPosition.y, planetName,false));
                }
                else //If planets are not allowed to repeat, then instantiate a randomly selected planet at the randomly selected coordinates and then remove both the planet and the coordinates from their respective lists to ensure niether is used again
                {
                    GameObject planetTemp;
                    planetTemp = Instantiate(allowablePlanets[randPlanetIndex], starField.CellToWorld(availableSpawnPoints[randSpawnIndex]), Quaternion.identity);
                    availableSpawnPoints.RemoveAt(randSpawnIndex);
                    allowablePlanets.RemoveAt(randPlanetIndex);
                    string clone = "(Clone)";
                    string planetName = planetTemp.name;
                    Vector3Int planetPosition = starField.WorldToCell(planetTemp.transform.position);
                    planetName = planetName.Replace(clone, "");
                    spawnedPlanets.Add(new PlanetObject(planetPosition.x, planetPosition.y, planetName,false));
                }
            }
        }
        int randStarGateIndex = Random.Range(0, availableSpawnPoints.Count);
        starGateSpawnPoint = availableSpawnPoints[randStarGateIndex];
        Instantiate(starGate, starField.CellToWorld(starGateSpawnPoint),Quaternion.identity);
        Save();
    }

    public void SpawnEnemies(int maxEnemies, bool repeatEnemies, List<int> allowedEnemies)
    {
        /*
         * This script is designed to spawn enemies randomly throughout the revealed sections of the map. It takes the following inputs
         * maxEnemies: This is a parameter that defines how many enemies will be spawned (Note: this parameter will be overruled in the case that the available spaces to spawn planets is less than this number)
         * repeatPlanets: This Boolean defines whether a planet is allowed to repeat or not
         * allowedPlanets: This is a list of the planets that the function can choose from when spawning the planets
         */
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); //create a list of any enemies that are currently in the scene
        List<GameObject> availableEnemies = new List<GameObject>() { enemyA, enemyB }; //create a list of available enemies based on supplied prefabs
        List<GameObject> allowableEnemies = new List<GameObject>(); //create a placeholder list for enemies that the function is allowed to spawn
        List<Vector3Int> availableSpawnPoints = new List<Vector3Int>(); //create a placeholder list for the available spawn points

        foreach (GameObject enemy in enemies) //loop through the list of any enemies currently in the scene and destroy them
        {
            GameObject.Destroy(enemy);
        }
        spawnedEnemies.Clear();

        foreach (int enemy in allowedEnemies) //loop through the allowed enemies supplied to the function and build a list of enemies that the function is allowed to spawn
        {
            allowableEnemies.Add(availableEnemies[enemy]);
        }

        int spawnRange = 2;
        if (maxEnemies <= 10){ spawnRange = 3; }
        else if (maxEnemies > 10 && maxEnemies <= 30){ spawnRange = 4; }
        else if (maxEnemies > 30 && maxEnemies <= 40){ spawnRange = 5; }
        else if (maxEnemies > 40 && maxEnemies <= 60){ spawnRange = 6; }
        else if (maxEnemies > 60 && maxEnemies <= 80){ spawnRange = 7; }
        else if (maxEnemies > 80 ){ spawnRange = 8; }


        for (int x = -spawnRange; x <= spawnRange; x++) //iterate through the range of the laser to generate the x coordinates
        {
            for (int y = -spawnRange; y <= spawnRange; y++) //iterate through the range of the laser to generate the y coordindates
            {
                Vector3Int tempCell = movementController.playerCellPosition + new Vector3Int(x, y, 0);
                float hexCellDistance = HexCellDistance(movementController.playerCellPositionCubeCoords, evenq2cube(tempCell));
                bool cellUnavailable = false;
                if (hexCellDistance <= spawnRange)
                {
                    foreach (GameObject enemy in enemies)
                    {
                        if (tempCell == gridLayout.WorldToCell(enemy.transform.position))
                        {
                            cellUnavailable = true;
                        }
                    }
                    if (hexCellDistance == 1 || tempCell.x > mapXMax || tempCell.x < mapXMin || tempCell.y > mapYMax || tempCell.y < mapYMin)
                    {
                        cellUnavailable = true;
                    }
                    if (!cellUnavailable)
                    {
                        availableSpawnPoints.Add(tempCell);
                    }
                }
        
            }
        }



        //availableSpawnPoints.AddRange(revealedTilesUnique); //populate the list of avaiable spawn points by adding all of the currently revelaed tiles
        availableSpawnPoints.Remove(playerState.playerCellPosition); //remove the players current position from the list of available spawn points so that enemies do not spawn beneath the player
        int maxNoRepeatEnemies = allowableEnemies.Count; //set the maximum number of enemies that can be spawned if they are not allowed to repeat
        int maxSpawnPointsAvailable = availableSpawnPoints.Count; //set the maxium number of spawn points that enemies can be spawned at

        for (int enemiesSpawned = 0; enemiesSpawned <= maxEnemies; enemiesSpawned++) //set up a loop to spawn enemies until the max value is reached
        {
            if ((!repeatEnemies && enemiesSpawned >= maxNoRepeatEnemies)||(enemiesSpawned >= maxSpawnPointsAvailable)) //set up an alternate termination condition in the event that spawned enemies exceed either the no repeat condition or the available spawn points
            {
                break;
            }
            else
            {
                int randEnemyIndex = Random.Range(0, allowableEnemies.Count); //generate a random index to be used to select which enemy will be spawned
                int randSpawnIndex = Random.Range(0, availableSpawnPoints.Count); //generate a random index to be used to select the spawn position of the enemy
                GameObject tempEnemy;
                if (repeatEnemies) //if enemies are allowed to repeat, then instantiate a random enemy at the randomly selected coordinates and thne remove those coordinates from the list of available coordinates
                {
                    tempEnemy = Instantiate(allowableEnemies[randEnemyIndex], starField.CellToWorld(availableSpawnPoints[randSpawnIndex]), Quaternion.identity);
                    availableSpawnPoints.RemoveAt(randSpawnIndex);

                    string clone = "(Clone)";
                    string enemyName = tempEnemy.name;
                    Vector3Int enemyPosition = starField.WorldToCell(tempEnemy.transform.position);
                    enemyName = enemyName.Replace(clone, "");
                    EnemyObject enemyObjectRef = new EnemyObject(enemyPosition.x, enemyPosition.y, enemyName);
                    spawnedEnemies.Add(new EnemyObject(enemyPosition.x, enemyPosition.y, enemyName));
                    tempEnemy.gameObject.GetComponent<EnemyShipControl>().thisEnemyObject = enemyObjectRef;
                }
                else //if enemies are not allowed to repeat, then instantiate a random enemy at the randomly selected coordinates and thne remove both the enemy and those coordinates from their respective lists 
                {
                    tempEnemy = Instantiate(allowableEnemies[randEnemyIndex], starField.CellToWorld(availableSpawnPoints[randSpawnIndex]), Quaternion.identity);
                    availableSpawnPoints.RemoveAt(randSpawnIndex);
                    allowableEnemies.RemoveAt(randEnemyIndex);

                    string clone = "(Clone)";
                    string enemyName = tempEnemy.name;
                    Vector3Int enemyPosition = starField.WorldToCell(tempEnemy.transform.position);
                    enemyName = enemyName.Replace(clone, "");
                    EnemyObject enemyObjectRef = new EnemyObject(enemyPosition.x, enemyPosition.y, enemyName);
                    spawnedEnemies.Add(enemyObjectRef);
                    tempEnemy.gameObject.GetComponent<EnemyShipControl>().thisEnemyObject = enemyObjectRef;
                }
            }
        }
    }

    public void GenericSpawnPlanets() //this function is intended to be the most basic application of the spawn planets function to be used for testing
    {
        List<int> allowedPlaents = new List<int>() {0,1,2,3,4,5,6,7,8,9,10};
        //List<int> allowedPlaents = new List<int>() {5,8};
        SpawnPlanets(mapXMax - 10, mapYMax - 10, mapXMin + 10, mapYMin + 10, false, allowedPlaents, 20);
    }
    public void ContextualSpawnPlanets()
    {
        int solarSystemNumber;
        if (QuickSaveRoot.Exists(resourceAndUpgradeManager.ResourceAndUpgradeDataSaveFileName))
        {
            QuickSaveReader instReader = QuickSaveReader.Create(resourceAndUpgradeManager.ResourceAndUpgradeDataSaveFileName);
            solarSystemNumber = instReader.Read<int>("solarSystemNumber");
        }
        else
        {
            solarSystemNumber = 1;
        }
        List<int> allowedPlaents;
        switch (solarSystemNumber)
        {
            case 1:
                allowedPlaents = new List<int>() { 0, 1, 2 };
                SpawnPlanets(mapXMax - 10, mapYMax - 10, mapXMin + 10, mapYMin + 10, true, allowedPlaents, 10);
                break;
            case 2:
                allowedPlaents = new List<int>() { 0, 1, 2, 3 };
                SpawnPlanets(mapXMax - 10, mapYMax - 10, mapXMin + 10, mapYMin + 10, true, allowedPlaents, 10);
                break;
            case 3:
                allowedPlaents = new List<int>() { 0, 1, 2, 3, 4 };
                SpawnPlanets(mapXMax - 10, mapYMax - 10, mapXMin + 10, mapYMin + 10, true, allowedPlaents, 10);
                break;
            case 4:
                allowedPlaents = new List<int>() { 0, 1, 2, 3, 4, 5 };
                SpawnPlanets(mapXMax - 10, mapYMax - 10, mapXMin + 10, mapYMin + 10, true, allowedPlaents, 10);
                break;
            case 5:
                allowedPlaents = new List<int>() { 0, 1, 2, 3, 4, 5, 6 };
                SpawnPlanets(mapXMax - 10, mapYMax - 10, mapXMin + 10, mapYMin + 10, true, allowedPlaents, 12);
                break;
            case 6:
                allowedPlaents = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
                SpawnPlanets(mapXMax - 10, mapYMax - 10, mapXMin + 10, mapYMin + 10, true, allowedPlaents, 12);
                break;
            case 7:
                allowedPlaents = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
                SpawnPlanets(mapXMax - 10, mapYMax - 10, mapXMin + 10, mapYMin + 10, true, allowedPlaents, 12);
                break;
            case 8:
                allowedPlaents = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                SpawnPlanets(mapXMax - 10, mapYMax - 10, mapXMin + 10, mapYMin + 10, true, allowedPlaents, 13);
                break;
            case 9:
                allowedPlaents = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                SpawnPlanets(mapXMax - 10, mapYMax - 10, mapXMin + 10, mapYMin + 10, true, allowedPlaents, 13);
                break;
            default:
                allowedPlaents = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                SpawnPlanets(mapXMax - 10, mapYMax - 10, mapXMin + 10, mapYMin + 10, true, allowedPlaents, 15);
                break;
        }
    }

    public void GenericSpawnEnemies() //this function is intended to be the most basic application of the spawn enemies function to be used for testing
    {
        List<int> allowedEnemies = new List<int>() { 0, 1};
        SpawnEnemies(2, true, allowedEnemies);
    }

    public void ContextualSpawnEnemies()
    {

        int maxEnemies;
        int solarSystemNumber;
        if (QuickSaveRoot.Exists(resourceAndUpgradeManager.ResourceAndUpgradeDataSaveFileName))
        {
            QuickSaveReader instReader = QuickSaveReader.Create(resourceAndUpgradeManager.ResourceAndUpgradeDataSaveFileName);
            solarSystemNumber = instReader.Read<int>("solarSystemNumber");
        }
        else
        {
            solarSystemNumber = 1;
        }

        switch (solarSystemNumber)
        {
            case 1:
                maxEnemies = 10;
                break;
            case 2:
                maxEnemies = 12;
                break;
            case 3:
                maxEnemies = 15;
                break;
            case 4:
                maxEnemies = 17;
                break;
            case 5:
                maxEnemies = 20;
                break;
            case 6:
                maxEnemies = 22;
                break;
            case 7:
                maxEnemies = 25;
                break;
            case 8:
                maxEnemies = 27;
                break;
            case 9:
                maxEnemies = 30;
                break;
            default:
                maxEnemies = 35;
                break;
        }

        List<int> allowedEnemies = new List<int>() { 0, 1 };

        if (resourceAndUpgradeManager.ThreatLevel <= 0.2)
        {
            //SpawnEnemies(2, true, allowedEnemies);
        }
        else
        {
            SpawnEnemies(Mathf.RoundToInt(maxEnemies*resourceAndUpgradeManager.ThreatLevel), true, allowedEnemies);
        }
    }

    public List<PlanetObject> UndiscoveredPlanets( List<PlanetObject> spawnedPlanets, List<Vector3Int> revealedTilesUnique)
    {
        List<PlanetObject> undiscPlanet = new List<PlanetObject>();
        
        foreach (Vector3Int tile in revealedTilesUnique)
        {
            foreach (PlanetObject planet in spawnedPlanets)
            {
                if(planet.xCoordinate == tile.x && planet.yCoordinate == tile.y)
                {
                    undiscPlanet.Add(planet);
                }
            }
        }
        return undiscPlanet;
    }

    public void OrderEnemyTurns()
    {
        GameObject[] enemyGameObjects;
        enemyGameObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemyGameObjects)
        {
            enemy.GetComponent<EnemyShipControl>().TakeTurn();
        }
        uiController.SetEndTurnButtonState();
    }

    public void ClearHighlighting()
    {
        //this operation will function almost identically to clearing the fog of war, except that it will highlight the cells within range of the laser. 
        for (int x = mapXMin; x <= mapYMax; x++) //iterate through the range of the laser to generate the x coordinates
        {
            for (int y = mapYMin; y <= mapYMax; y++) //iterate through the range of the laser to generate the y coordindates
            {
                highlightWeaponMap.SetTile(new Vector3Int(x, y, 0), null); //set the cell at the current coordinates to null
            }
        }
        //Debug.Log("cleared highlighted list");
        currentHighlightedTiles.Clear(); //clear the list of highlighted cell coordinates.
    }

    public List<Vector3Int> GetFlats(int flatLength, Vector3Int centerPoint, bool player)
    {
        List<Vector3Int> flats = new List<Vector3Int>();
        if (player)
        {
            Vector3Int tempHexCalc = new Vector3Int(centerPoint.x + flatLength, centerPoint.y, centerPoint.z);
            flats.Add(tempHexCalc);
            tempHexCalc = new Vector3Int(centerPoint.x - flatLength, centerPoint.y, centerPoint.z);
            flats.Add(tempHexCalc);
            if (centerPoint.y % 2 == 0)
            {
                if (flatLength % 2 == 0)
                {
                    tempHexCalc = new Vector3Int(centerPoint.x + Mathf.FloorToInt(flatLength / 2), centerPoint.y - flatLength, centerPoint.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(centerPoint.x - Mathf.FloorToInt(flatLength / 2), centerPoint.y - flatLength, centerPoint.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(centerPoint.x + Mathf.FloorToInt(flatLength / 2), centerPoint.y + flatLength, centerPoint.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(centerPoint.x - Mathf.FloorToInt(flatLength / 2), centerPoint.y + flatLength, centerPoint.z);
                    flats.Add(tempHexCalc);
                }
                else
                {
                    tempHexCalc = new Vector3Int(centerPoint.x + Mathf.FloorToInt(flatLength / 2), centerPoint.y - flatLength, centerPoint.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(centerPoint.x - Mathf.FloorToInt(flatLength / 2) - 1, centerPoint.y - flatLength, centerPoint.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(centerPoint.x + Mathf.FloorToInt(flatLength / 2), centerPoint.y + flatLength, centerPoint.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(centerPoint.x - Mathf.FloorToInt(flatLength / 2) - 1, centerPoint.y + flatLength, centerPoint.z);
                    flats.Add(tempHexCalc);
                }

            }
            else
            {
                if (flatLength % 2 == 0)
                {
                    tempHexCalc = new Vector3Int(centerPoint.x + Mathf.FloorToInt(flatLength / 2), centerPoint.y - flatLength, centerPoint.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(centerPoint.x - Mathf.FloorToInt(flatLength / 2), centerPoint.y - flatLength, centerPoint.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(centerPoint.x + Mathf.FloorToInt(flatLength / 2), centerPoint.y + flatLength, centerPoint.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(centerPoint.x - Mathf.FloorToInt(flatLength / 2), centerPoint.y + flatLength, centerPoint.z);
                    flats.Add(tempHexCalc);
                }
                else
                {
                    tempHexCalc = new Vector3Int(centerPoint.x + Mathf.FloorToInt(flatLength / 2) + 1, centerPoint.y - flatLength, centerPoint.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(centerPoint.x - Mathf.FloorToInt(flatLength / 2), centerPoint.y - flatLength, centerPoint.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(centerPoint.x + Mathf.FloorToInt(flatLength / 2) + 1, centerPoint.y + flatLength, centerPoint.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(centerPoint.x - Mathf.FloorToInt(flatLength / 2), centerPoint.y + flatLength, centerPoint.z);
                    flats.Add(tempHexCalc);
                }

            }
            //mapManager.ClearHighlighting();
            //mapManager.HighlightSet(flats, true);
        }
        else
        {
            for (int i = 0; i <= flatLength - 1; i++)
            {
                Vector3Int tempHexCalc = new Vector3Int(centerPoint.x + i + 1, centerPoint.y, centerPoint.z);
                flats.Add(tempHexCalc);
                tempHexCalc = new Vector3Int(centerPoint.x - i - 1, centerPoint.y, centerPoint.z);
                flats.Add(tempHexCalc);
                if (centerPoint.y % 2 == 0)
                {
                    if (i % 2 == 0)
                    {
                        tempHexCalc = new Vector3Int(centerPoint.x + Mathf.FloorToInt(i / 2), centerPoint.y - i - 1, centerPoint.z);
                        flats.Add(tempHexCalc);
                        tempHexCalc = new Vector3Int(centerPoint.x - Mathf.FloorToInt(i / 2) - 1, centerPoint.y - i - 1, centerPoint.z);
                        flats.Add(tempHexCalc);
                        tempHexCalc = new Vector3Int(centerPoint.x + Mathf.FloorToInt(i / 2), centerPoint.y + i + 1, centerPoint.z);
                        flats.Add(tempHexCalc);
                        tempHexCalc = new Vector3Int(centerPoint.x - Mathf.FloorToInt(i / 2) - 1, centerPoint.y + i + 1, centerPoint.z);
                        flats.Add(tempHexCalc);
                    }
                    else
                    {
                        tempHexCalc = new Vector3Int(centerPoint.x + Mathf.FloorToInt(i / 2) + 1, centerPoint.y - i - 1, centerPoint.z);
                        flats.Add(tempHexCalc);
                        tempHexCalc = new Vector3Int(centerPoint.x - Mathf.FloorToInt(i / 2) - 1, centerPoint.y - i - 1, centerPoint.z);
                        flats.Add(tempHexCalc);
                        tempHexCalc = new Vector3Int(centerPoint.x + Mathf.FloorToInt(i / 2) + 1, centerPoint.y + i + 1, centerPoint.z);
                        flats.Add(tempHexCalc);
                        tempHexCalc = new Vector3Int(centerPoint.x - Mathf.FloorToInt(i / 2) - 1, centerPoint.y + i + 1, centerPoint.z);
                        flats.Add(tempHexCalc);
                    }

                }
                else
                {
                    if (i % 2 == 0)
                    {
                        tempHexCalc = new Vector3Int(centerPoint.x + Mathf.FloorToInt(i / 2) + 1, centerPoint.y - i - 1, centerPoint.z);
                        flats.Add(tempHexCalc);
                        tempHexCalc = new Vector3Int(centerPoint.x - Mathf.FloorToInt(i / 2), centerPoint.y - i - 1, centerPoint.z);
                        flats.Add(tempHexCalc);
                        tempHexCalc = new Vector3Int(centerPoint.x + Mathf.FloorToInt(i / 2) + 1, centerPoint.y + i + 1, centerPoint.z);
                        flats.Add(tempHexCalc);
                        tempHexCalc = new Vector3Int(centerPoint.x - Mathf.FloorToInt(i / 2), centerPoint.y + i + 1, centerPoint.z);
                        flats.Add(tempHexCalc);
                    }
                    else
                    {
                        tempHexCalc = new Vector3Int(centerPoint.x + Mathf.FloorToInt(i / 2) + 1, centerPoint.y - i - 1, centerPoint.z);
                        flats.Add(tempHexCalc);
                        tempHexCalc = new Vector3Int(centerPoint.x - Mathf.FloorToInt(i / 2) - 1, centerPoint.y - i - 1, centerPoint.z);
                        flats.Add(tempHexCalc);
                        tempHexCalc = new Vector3Int(centerPoint.x + Mathf.FloorToInt(i / 2) + 1, centerPoint.y + i + 1, centerPoint.z);
                        flats.Add(tempHexCalc);
                        tempHexCalc = new Vector3Int(centerPoint.x - Mathf.FloorToInt(i / 2) - 1, centerPoint.y + i + 1, centerPoint.z);
                        flats.Add(tempHexCalc);
                    }

                }

            }
        }

        if (player)
        {
            foreach (Vector3Int flat in flats)
            {
                //Debug.Log(flat);
            }
        }


        return flats;
    }

    public void ShowFlats(string objectName, Vector3Int cellPosition, GameObject givenObject)
    {
        List<Vector3Int> flats = new List<Vector3Int>();
        if (objectName == "EnemyA")
        {
            flats = GetFlats(3, cellPosition, false);
            givenObject.GetComponent<EnemyShipControl>().highlightEnabled = !givenObject.GetComponent<EnemyShipControl>().highlightEnabled;
            HighlightSet(flats, givenObject.GetComponent<EnemyShipControl>().highlightEnabled);
        }
        else if (objectName == "EnemyB")
        {
            flats = GetFlats(1, cellPosition, false);
            givenObject.GetComponent<EnemyShipControl>().highlightEnabled = !givenObject.GetComponent<EnemyShipControl>().highlightEnabled;
            HighlightSet(flats, givenObject.GetComponent<EnemyShipControl>().highlightEnabled);
        }
        else if (objectName == "Player")
        {
            
        }

        

        
    }

    public void ShowFlats(string objectName, Vector3Int cellPosition, GameObject givenObject, bool state)
    {
        List<Vector3Int> flats = new List<Vector3Int>();
        if (objectName == "EnemyA")
        {
            flats = GetFlats(3, cellPosition, false);
            givenObject.GetComponent<EnemyShipControl>().highlightEnabled = state;
        }
        else if (objectName == "EnemyB")
        {
            flats = GetFlats(1, cellPosition, false);
            givenObject.GetComponent<EnemyShipControl>().highlightEnabled = state;
        }
        else if (objectName == "Player")
        {

        }

        

        HighlightSet(flats, state);
    }

    public List<Vector3Int> GetNeighbours(Vector3Int origin)
    {
        List<Vector3Int> openNeighbours = new List<Vector3Int>();
        bool setSkip = false;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int modX = origin.x + x;
                int modY = origin.y + y;

                if (modX < mapXMax && modX > mapXMin && modY < mapYMax && modY > mapYMin)
                {


                    if (HexCellDistance(evenq2cube(origin), evenq2cube(new Vector3Int(modX, modY, 0))) <= 1)
                    {
                        foreach (EnemyObject fellowEnemy in spawnedEnemies)
                        {
                            if (fellowEnemy.xCoordinate == modX && fellowEnemy.yCoordinate == modY)
                            {
                                setSkip = true;
                            }


                        }
                        if (!setSkip)
                        {
                            openNeighbours.Add(new Vector3Int(modX, modY, 0));
                        }
                        else
                        {
                            setSkip = false;
                        }

                    }


                }
            }
        }

        return openNeighbours;
    }
}
