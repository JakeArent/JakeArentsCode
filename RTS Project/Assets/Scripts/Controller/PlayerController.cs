using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    // Use this for initialization
    

    public List<GameObject> SelectedEntities = new List<GameObject>();
    public List<NetworkInstanceId> SelectedId = new List<NetworkInstanceId>();

    [SyncVar]
    public int Team;

    public GameObject SelectionText;
    public GameObject BuildingNameText;
    public GameObject QueueText;
    public GameObject GoldText;
    public GameObject ManaText;

    private bool SelectingArea = false;
    private Vector3 Pos1;
    private Vector3 Pos2;

    private Vector3 MoveWaypoint;
    private bool mvCommand = false;
    private bool AboutToAMove = false;

    private bool PlacingBuilding = false;
    private GameObject BuildingPlaceGhost = null;

    //building variables
    public GameObject UnitCanv;
    public GameObject BuildCanv;
    public GameObject WorkerCanv;
    public List<Button> CreationButtons = new List<Button>();
    public List<Button> WorkerButtons = new List<Button>();

    //Units and Resources
    public List<GameObject> Team1Units = new List<GameObject>();
    public List<GameObject> Team2Units = new List<GameObject>();

    public List<GameObject> Team1Buildings = new List<GameObject>();
    public List<GameObject> Team2Buildings = new List<GameObject>();

    public List<float> ResourceIncome = new List<float>();


    public int Gold = 0;

    public float Efficiency = 0;

    void Start ()
    {
        if (isLocalPlayer)
        {
            transform.position = Camera.main.transform.position;
            Camera.main.transform.parent = transform;
            
            transform.GetChild(0).gameObject.SetActive(true);

            Team = GameObject.FindGameObjectsWithTag("Player").Length;
            if (Team <= 0)
                Team = 1;
                
            GameObject[] bases = GameObject.FindGameObjectsWithTag("Base");
            Vector3 offset = new Vector3(5, 5.5f, -5);
            foreach (GameObject g in bases)
            {
                if (g.GetComponent<Selectable>().Team == Team)
                {
                    transform.position = g.transform.position + offset;
                }
            }
            ResourceIncome.Add(0);
            ResourceIncome.Add(0);
            Gold = 100;
            StartCoroutine(ResourceUp());
        }
        StartCoroutine(UpdateRegistry());
    }

    public IEnumerator ResourceUp()
    {
        while (true)
        {
            //Update resources
            //Team 1
            ResourceIncome[0] = 0;
            ResourceIncome[1] = 5;
            float ManaExpenditure = 0;
            if (Team == 1)
            {
                foreach (GameObject building in Team1Buildings)
                {
                    if (building.GetComponent<ProductionBuilding>() != null)
                    {
                        ProductionBuilding pb = building.GetComponent<ProductionBuilding>();
                        if (pb.ResourceProduced == 0)
                        {
                            //this is gold
                            ResourceIncome[0] += ((float)pb.WorkersGarrisoned / (float)pb.MaxWorkersGarrisoned) * pb.ResourceQuantity;
                        }
                        else if (pb.ResourceProduced == 1)
                        {
                            ResourceIncome[1] += (pb.WorkersGarrisoned / pb.MaxWorkersGarrisoned) * pb.ResourceQuantity;
                        }
                    }
                    else if (building.GetComponent<Building>().ProductionQueue.Count > 0)
                    {
                        ManaExpenditure += building.GetComponent<Building>().EnergyConsumption;
                    }
                }
            }
            else if (Team == 2)
            {
                //Team 2
                ResourceIncome[0] = 0;
                ResourceIncome[1] = 5;
                float team2energy = 0;
                foreach (GameObject building in Team2Buildings)
                {
                    if (building.GetComponent<ProductionBuilding>() != null)
                    {
                        ProductionBuilding pb = building.GetComponent<ProductionBuilding>();
                        if (pb.ResourceProduced == 0)
                        {
                            //this is gold
                            ResourceIncome[0] += (float)(pb.WorkersGarrisoned / pb.MaxWorkersGarrisoned) * pb.ResourceQuantity;
                        }
                        else if (pb.ResourceProduced == 1)
                        {
                            ResourceIncome[1] += (pb.WorkersGarrisoned / pb.MaxWorkersGarrisoned) * pb.ResourceQuantity;
                        }
                    }
                    else if (building.GetComponent<Building>().ProductionQueue.Count > 0)
                    {
                        team2energy += building.GetComponent<Building>().EnergyConsumption;
                    }
                }
            }
            //add gold
            Gold += Mathf.RoundToInt(ResourceIncome[0]);
            //Get Efficiency
            Efficiency = (ResourceIncome[1] / ManaExpenditure) * 100;
            if (ManaExpenditure == 0 || Efficiency > 100)
                Efficiency = 100;

            yield return new WaitForSeconds(1);
        }
    }

    public IEnumerator UpdateRegistry()
    {
        while (true)
        {
            //prune empty and destroyed units
            
            yield return new WaitForSeconds(1);
            GameObject[] things = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject g in things)
            {
                if (g.GetComponent<Unit>() != null)
                {
                    if (g.GetComponent<Unit>().Team == 1)
                    {
                        if (!Team1Units.Contains(g))
                            Team1Units.Add(g);
                    }
                    else if (g.GetComponent<Unit>().Team == 2)
                    {
                        if (!Team2Units.Contains(g))
                            Team2Units.Add(g);
                    }
                }
                else if (g.GetComponent<Building>() != null)
                {
                    if (g.GetComponent<Building>().Team == 1)
                    {
                        if (!Team1Buildings.Contains(g))
                            Team1Buildings.Add(g);
                    }
                    else if (g.GetComponent<Building>().Team == 2)
                    {
                        if (!Team2Buildings.Contains(g))
                            Team2Buildings.Add(g);
                    }
                }
            }
        }
    }

    public void DelUnit(GameObject unit)
    {
        if (Team1Units.Contains(unit))
            Team1Units.Remove(unit);
        else if (Team2Units.Contains(unit))
            Team2Units.Remove(unit);
        else if (Team1Buildings.Contains(unit))
            Team1Buildings.Remove(unit);
        else if (Team2Buildings.Contains(unit))
            Team2Buildings.Remove(unit);
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            //purge any dead selected units
            try
            {
                foreach (GameObject g in SelectedEntities)
                {
                    if (g == null)
                        SelectedEntities.Remove(g);
                    else if (g.GetComponent<Selectable>().health <= 0)
                    {
                        SelectedEntities.Remove(g);
                    }
                }
            }
            catch (System.InvalidOperationException e)
            {
                Debug.Log("GUI was Iterating when the unit was killed");
            }

            //Run if units are selected
            GameObject w = null;
            if (UnitCanv.activeInHierarchy)
            {
                SelectionText.GetComponent<Text>().text = "Selected Units:\n";
                //Update GUI
                int[] Units = new int[4];
                for (int i = 0; i < SelectedEntities.Count; i++)
                {
                    switch (SelectedEntities[i].GetComponent<Selectable>().Name)
                    {
                        default:
                            Units[0]++;
                            w = SelectedEntities[i];
                            break;
                        case "Swordsman":
                            Units[1]++;
                            break;
                        case "Archer":
                            Units[2]++;
                            break;
                        case "Spearman":
                            Units[3]++;
                            break;
                    }
                }
                //add numbers to GUI
                if (Units[0] > 0)
                    SelectionText.GetComponent<Text>().text += "Workers: " + Units[0] + "\n";
                if (Units[1] > 0)
                    SelectionText.GetComponent<Text>().text += "Swordsmen: " + Units[1] + "\n";
                if (Units[2] > 0)
                    SelectionText.GetComponent<Text>().text += "Archers: " + Units[2] + "\n";
                if (Units[3] > 0)
                    SelectionText.GetComponent<Text>().text += "Spearmen: " + Units[3] + "\n";

                //if a worker is selected, turn on the worker canvas
                if (Units[0] > 0)
                {
                    WorkerCanv.SetActive(true);
                    //update the buttons
                    for (int i = 0; i < WorkerButtons.Count; i++)
                    {
                        WorkerButtons[i].gameObject.SetActive(true);
                        WorkerButtons[i].gameObject.GetComponentInChildren<Text>().text = w.GetComponent<Worker>().BuildableBuildings[i].GetComponent<Building>().Name;
                        WorkerButtons[i].gameObject.GetComponentInChildren<Text>().text += "\n" + w.GetComponent<Worker>().BuildableBuildings[i].GetComponent<Building>().Cost + " Gold";
                        if (Gold < w.GetComponent<Worker>().BuildableBuildings[i].GetComponent<Building>().Cost)
                        {
                            WorkerButtons[i].GetComponent<Image>().color = Color.red;
                        }
                        else
                            WorkerButtons[i].GetComponent<Image>().color = Color.white;
                    }
                }
                else
                {
                    WorkerCanv.SetActive(false);
                }
            }
            //run if a building is selected
            else if (BuildCanv.activeInHierarchy && SelectedEntities.Count > 0)
            {
                //set building name
                Building b = SelectedEntities[0].GetComponent<Building>();
                string newText = "Queue: ";
                if (b.ProductionQueue.Count > 0)
                    newText += ((b.ProductionProgress / b.ProductionTime) * 100).ToString() + "%";
                newText += "\n";
                foreach (GameObject unit in b.ProductionQueue)
                {
                    newText += unit.GetComponent<Unit>().Name + "\n";
                }
                QueueText.GetComponent<Text>().text = newText;

                //update Building buttons
                for (int i = 0; i < CreationButtons.Count; i++)
                {
                    if (i < b.BuildableUnits.Count)
                    {
                        CreationButtons[i].gameObject.SetActive(true);
                        CreationButtons[i].gameObject.GetComponentInChildren<Text>().text = b.BuildableUnits[i].GetComponent<Unit>().Name;
                        if (SelectedEntities[0].GetComponent<ProductionBuilding>() == null)
                            CreationButtons[i].gameObject.GetComponentInChildren<Text>().text += "\n" + b.BuildableUnits[i].GetComponent<Unit>().Cost + " Gold";
                        if (Gold < b.BuildableUnits[i].GetComponent<Unit>().Cost)
                        {
                            CreationButtons[i].GetComponent<Image>().color = Color.red;
                        }
                        else
                            CreationButtons[i].GetComponent<Image>().color = Color.white;
                    }
                    else
                        CreationButtons[i].gameObject.SetActive(false);

                    //if its a production building, ignore this
                    if (b.gameObject.GetComponent<ProductionBuilding>() != null && b.gameObject.GetComponent<ProductionBuilding>().WorkersGarrisoned <= i)
                    {

                        CreationButtons[i].gameObject.SetActive(false);
                    }
                }
            }
            //update eco info
            GoldText.GetComponent<Text>().text = "Gold: " + Gold;
            ManaText.GetComponent<Text>().text = "Mana: " + ResourceIncome[1] + "\n\nEfficiency: " + Mathf.RoundToInt(Efficiency) + "%";

            //Run if placing a building
            if (PlacingBuilding)
            {
                Debug.Log("Placing");
                LayerMask mask = LayerMask.GetMask("Terrain");
                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit h;
                if (Physics.Raycast(r, out h, Mathf.Infinity, mask))
                {
                    if (BuildingPlaceGhost.GetComponent<ProductionBuilding>() != null)
                    {
                        GameObject[] Nodes;
                        if (BuildingPlaceGhost.GetComponent<ProductionBuilding>().Name == "Mine")
                        {
                            Nodes = GameObject.FindGameObjectsWithTag("Gold Node");
                        }
                        else
                        {
                            Nodes = GameObject.FindGameObjectsWithTag("Mana Node");
                        }
                        GameObject closestNode = Nodes[0];
                        foreach (GameObject n in Nodes)
                        {
                            if (Vector3.Distance(h.point, n.transform.position) < Vector3.Distance(h.point, closestNode.transform.position))
                            {
                                closestNode = n;
                            }
                        }
                        BuildingPlaceGhost.transform.position = closestNode.transform.position;
                    }
                    else
                    {
                        BuildingPlaceGhost.transform.position = h.point;
                    }

                    //if placing a production building, instead snap-to a grid.
                }
            }
            //start selection methods
            if (Input.GetKeyDown(KeyCode.A))
                AboutToAMove = true;
            if (Input.GetKeyDown(KeyCode.S))
            {
                AboutToAMove = false;
                foreach (GameObject g in SelectedEntities)
                {
                    Cmd_GiveCommand("STOP", g, g.transform.position, null);
                }
            }
            Select();
            //check for movement command
            if (Input.GetAxisRaw("Fire2") > 0 && !mvCommand)
            {
                Debug.Log("RIGHT CLIKCI");
                if (PlacingBuilding)
                {
                    Gold += BuildingPlaceGhost.GetComponent<Building>().Cost;
                    Destroy(BuildingPlaceGhost);
                    BuildingPlaceGhost = null;
                    PlacingBuilding = false;
                }
                else
                {
                    mvCommand = true;
                }

            }
            if (Input.GetAxisRaw("Fire2") <= 0 && mvCommand)
            {
                mvCommand = false;
                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit h;
                if (Physics.Raycast(r, out h))
                {
                    if (h.transform.gameObject.GetComponent<ProductionBuilding>() != null && h.transform.gameObject.GetComponent<Selectable>().Team == Team)
                    {
                        foreach (GameObject g in SelectedEntities)
                        {
                            if (g.GetComponent<Worker>() != null && h.transform.gameObject.GetComponent<ProductionBuilding>().WorkersGarrisoned < h.transform.gameObject.GetComponent<ProductionBuilding>().MaxWorkersGarrisoned)
                                Cmd_GiveCommand("Garrison", g, h.point, h.transform.gameObject);
                        }
                    }
                    else
                    {
                        MoveWaypoint = h.point;
                        foreach (GameObject g in SelectedEntities)
                        {
                            if (AboutToAMove)
                                Cmd_GiveCommand("AMove", g, MoveWaypoint, h.transform.gameObject);
                            else
                                Cmd_GiveCommand("Move", g, MoveWaypoint, h.transform.gameObject);
                        }
                    }
                }
                AboutToAMove = false;
            }
        }
    
	}

    [Command]
    public void Cmd_GiveCommand(string cmdType, GameObject cmdRecv, Vector3 movPos, GameObject clicked)
    {
        switch(cmdType)
        {
            default:
                break;
            case "Move":
                if (cmdRecv.GetComponent<Unit>() != null)
                {
                    //set the destination and tell the unit to move
                    cmdRecv.GetComponent<Unit>().MovementWaypoint = movPos;
                    cmdRecv.GetComponent<Unit>().Move("Move");
                }
                else if (cmdRecv.GetComponent<Building>() != null)
                {
                    //set the buildings rally point;
                    cmdRecv.GetComponent<Building>().RallyPoint = movPos;
                }
                break;

            case "AMove":
                if(cmdRecv.GetComponent<Unit>() != null)
                {
                    //set the destination and tell the unit to move
                    cmdRecv.GetComponent<Unit>().MovementWaypoint = movPos;
                    cmdRecv.GetComponent<Unit>().Move("Attack Move");
                }
                else if (cmdRecv.GetComponent<Building>() != null)
                {
                    //set the buildings rally point;
                    cmdRecv.GetComponent<Building>().RallyPoint = movPos;
                }
                break;

            case "STOP":
                if (cmdRecv.GetComponent<Unit>() != null)
                {
                    cmdRecv.GetComponent<Unit>().agent.destination = movPos;
                    cmdRecv.GetComponent<Unit>().State = "Idle";
                }
                break;

            case "Garrison":
                cmdRecv.GetComponent<Unit>().MovementWaypoint = movPos;
                cmdRecv.GetComponent<Worker>().Garrison(clicked);
                break;
        }
    }

    [Command]
    public void Cmd_PlaceBuilding(GameObject building)
    {
        NetworkServer.Spawn(building);
        building.GetComponent<Building>().Team = Team;
    }

    public void Select()
    {
        if(Input.GetAxisRaw("Fire1") > 0 && !SelectingArea && !PlacingBuilding)
        {
            Pos1 = Input.mousePosition;
            SelectingArea = true;
            AboutToAMove = false;
        }
        else if (Input.GetAxisRaw("Fire1") > 0 && PlacingBuilding)
        {
            PlacingBuilding = false;
            foreach (GameObject w in SelectedEntities)
            {
                if (w.GetComponent<Worker>() != null)
                {
                    w.GetComponent<Worker>().Build(BuildingPlaceGhost);
                    break;
                }
            }

            BuildingPlaceGhost = null;
        }
        //get either a single unit or an area
        if (Input.GetAxisRaw("Fire1") <= 0 && SelectingArea)
        {
            Pos2 = Input.mousePosition;
            SelectingArea = false;
            float area = (Mathf.Abs(Pos1.x - Pos2.x) * 2) + (Mathf.Abs(Pos1.y - Pos2.y));
            if (area > 10) //select an area instead of a single unit if above the threshold
            {
                Ray r1 = Camera.main.ScreenPointToRay(Pos1);
                Ray r2 = Camera.main.ScreenPointToRay(Pos2);
                RaycastHit h1, h2;
                if (Physics.Raycast(r1, out h1) && Physics.Raycast(r2,out h2))
                {
                    SelectArea(h1.point, h2.point);
                }
            }
            else
            {
                //Select Single Unit
                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit h;
                if (Physics.Raycast(r, out h))
                {
                    if (h.transform.gameObject.GetComponent<Selectable>() != null)
                    {
                        if (h.transform.tag != "Terrain" && !SelectedId.Contains(h.transform.gameObject.GetComponent<NetworkIdentity>().netId) && h.transform.gameObject.GetComponent<Selectable>().Team == Team)
                        {
                            if (h.transform.GetComponent<Unit>() != null)
                            {
                                //set unit ui to true
                                UnitCanv.SetActive(true);
                                BuildCanv.SetActive(false);

                                /*
                                 Clear if:
                                 a. Not selecting multiple units
                                 b. A building is selected
                                 */
                                if (!Input.GetKey(KeyCode.LeftShift) || (SelectedEntities.Count >0 && SelectedEntities[0].GetComponent<Building>() != null))
                                    Deselect();
                                SelectedEntities.Add(h.transform.gameObject);
                                SelectedId.Add(h.transform.gameObject.GetComponent<NetworkIdentity>().netId);
                            }
                            else if (h.transform.GetComponent<Building>() != null)
                            {
                                //clear list and switch GUI
                                Deselect();
                                UnitCanv.SetActive(false);
                                WorkerCanv.SetActive(false);
                                BuildCanv.SetActive(true);

                                //add to lists
                                SelectedEntities.Add(h.transform.gameObject);
                                SelectedId.Add(h.transform.gameObject.GetComponent<NetworkIdentity>().netId);

                                //set up initial GUI
                                Building b = SelectedEntities[0].GetComponent<Building>();
                                BuildingNameText.GetComponent<Text>().text = b.Name;

                                //set up buttons
                                for (int i = 0; i < CreationButtons.Count; i++)
                                {
                                    if (i < b.BuildableUnits.Count)
                                    {
                                        CreationButtons[i].gameObject.SetActive(true);
                                        CreationButtons[i].gameObject.GetComponentInChildren<Text>().text = b.BuildableUnits[i].GetComponent<Unit>().Name;
                                    }
                                    else
                                        CreationButtons[i].gameObject.SetActive(false);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void SelectArea(Vector3 Pos1, Vector3 Pos2)
    {
        //set up the smallest and largest x/z values
        float LX, SX, LZ, SZ;
        LX = Pos1.x;
        SX = Pos1.x;
        if (Pos1.x < Pos2.x)
            LX = Pos2.x;
        else
            SX = Pos2.x;

        SZ = Pos1.z;
        LZ = Pos1.z;
        if (Pos1.z < Pos2.z)
            LZ = Pos2.z;
        else
            SZ = Pos2.z;

        Debug.Log("Bounds: ");
        Debug.Log("Lower X: " + SX);
        Debug.Log("Upper X: " + LX);
        Debug.Log("Lower Z: " + SZ);
        Debug.Log("Upper Z: " + LZ);

        //check for units inside this quad
        if (!Input.GetKey(KeyCode.LeftShift))
            Deselect();
        if (Team == 1)
        {
            Debug.Log("ITerating");
            foreach (GameObject unit in Team1Units)
            {
                Vector3 p = unit.transform.position;
                Debug.Log("Unit at " + p);
                if ((p.x >= SX && p.x <= LX) && (p.z >= SZ && p.z <= LZ))
                {
                    SelectedEntities.Add(unit);
                    SelectedId.Add(unit.GetComponent<NetworkIdentity>().netId);
                }
            }
        }
        else if (Team == 2)
        {
            foreach (GameObject unit in Team2Units)
            {
                Vector3 p = unit.transform.position;
                Debug.Log("Unit at " + p);
                if ((p.x >= SX && p.x <= LX) && (p.z >= SZ && p.z <= LZ))
                {
                    SelectedEntities.Add(unit);
                }
            }
        }
    }

    public void Deselect()
    {
        SelectedEntities.Clear();
        SelectedId.Clear();
    }

    public void ClickButton(int btn)
    {
        Building b = SelectedEntities[0].GetComponent<Building>();
        if (SelectedEntities[0].GetComponent<ProductionBuilding>() != null)
        {
            Cmd_SpawnWorker(SelectedEntities[0]);
        }
        else if (Gold >= b.BuildableUnits[btn].GetComponent<Unit>().Cost)
        {
            b.QueueUnit(b.BuildableUnits[btn]);
            Cmd_QueueUnit(SelectedEntities[0], btn);
            Gold -= b.BuildableUnits[btn].GetComponent<Unit>().Cost;
        }
    }

    public void ClickWorkerButton(int btn)
    {
        Worker worker = null;
        foreach (GameObject w in SelectedEntities)
        {
            if (w.GetComponent<Worker>() != null)
                worker = w.GetComponent<Worker>();
            break;
        }
        if (Gold >= worker.BuildableBuildings[btn].GetComponent<Building>().Cost)
        {
            BuildingPlaceGhost = Instantiate(worker.BuildableBuildings[btn]);
            Gold -= worker.BuildableBuildings[btn].GetComponent<Building>().Cost;
            PlacingBuilding = true;
        }
    }

    [Command]
    public void Cmd_SpawnUnit(GameObject g)
    {
        Debug.Log("SPAWN");
        g.GetComponent<Building>().SpawnUnit();
    }

    [Command]
    public void Cmd_QueueUnit(GameObject building, int thingToQueue)
    {
        building.GetComponent<Building>().QueueUnit(building.GetComponent<Building>().BuildableUnits[thingToQueue]);
    }

    [Command]
    public void Cmd_SpawnWorker(GameObject building)
    {
        //instantiate it
        GameObject u = Instantiate(building.GetComponent<ProductionBuilding>().WorkerPrefab, building.GetComponent<ProductionBuilding>().SpawnPoint.transform.position, Quaternion.identity);
        //network spawn
        NetworkServer.Spawn(u);
        //set team
        u.GetComponent<Selectable>().Team = building.GetComponent<Selectable>().Team;
        Debug.Log("Team set to " + Team);
        //Decrement the workers garrisoned
        building.GetComponent<ProductionBuilding>().WorkersGarrisoned--;
    }

    [Command]
    public void Cmd_SpawnBuilding(string n,Vector3 pos, GameObject source)
    {
        foreach (GameObject b in source.GetComponent<Worker>().BuildableBuildings)
        {
            if (b.GetComponent<Selectable>().Name == n)
            {
                GameObject build = Instantiate(b, pos, Quaternion.identity);
                NetworkServer.Spawn(build);
                build.GetComponent<Selectable>().Team = source.GetComponent<Selectable>().Team;
            }
        }
    }
}
