using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 using UnityEngine.EventSystems;

public class UIScript : MonoBehaviour
{
    public Text infectious, new_infections, slider_text, number_of_visits, number_of_vaccinated;
    public Button start, stop, edit;
    public InputField visitors_input, spawn_interval_input, inzidenz_input, length_input, upperBounds, lowerBounds, levelOfInterest;
    public Slider vaccinated;
    public Button boothButton, booth1Button, booth2Button, booth3Button, booth4Button, foodButton, hallwayButton, hallButton, innsersectionButton, spawnButton, exitButton;

    public GameObject booth, booth1, booth2, booth3, booth4, food, hallway, innsersection, hall, spawn, exit;

    public Dropdown maskDropdown;

    public Camera camera;

    private List<Button> differentBooths;
    private List<Button> hallElements;
    private float mouseWheelRotation;
    private GameObject currentPlacableObject, selectedObjectPlacable, selectedObject, selectedBuildingBlock, selectedBuildingBlockPlacable, currentBlock;
    private bool started;
    private int interval;
    private List<VisitorAgents_2> visitors;
    private List<SpawnScript> spawns;
    // Start is called before the first frame update
    private FlyCamera flyCamera;

    private string maskText;
    void Start()
    {
        started = false;
        interval = 120;
        visitors = new List<VisitorAgents_2>();
        spawns = GetSpawns();
        selectedObjectPlacable = booth;
        flyCamera = camera.GetComponent<FlyCamera>();

        selectedBuildingBlock = hallway;
        maskText = "None";
        assignButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
        UpdateUI();
        if(flyCamera.getActive()){
            LeftClickHandle();
        }
        RightClickHandle();
        HandleDel();
        HandleLeftDel();
        handleEsc();
        HandleF();

        if(currentPlacableObject != null){
            MoveCurrentObjectToMouse();
            RotateFromMouseWheel();
        }
    }

    private void assignButtons(){
        start.onClick.AddListener(onStartClick);
        stop.onClick.AddListener(onStopClick);

        boothButton.onClick.AddListener(onBoothClick);
        foodButton.onClick.AddListener(onFoodClick);
        booth1Button.onClick.AddListener(onBooth1Click);
        booth2Button.onClick.AddListener(onBooth2Click);
        booth3Button.onClick.AddListener(onBooth3Click);
        booth4Button.onClick.AddListener(onBooth4Click);

        hallwayButton.onClick.AddListener(onHallwayClick);
        hallButton.onClick.AddListener(onHallClick);
        innsersectionButton.onClick.AddListener(onSectionClick);
        spawnButton.onClick.AddListener(onSpawnClick);
        exitButton.onClick.AddListener(onExitClick);
        edit.onClick.AddListener(onEditClick);
        maskDropdown.onValueChanged.AddListener(delegate {onMaskValue(maskDropdown);});
    }


    private void UpdateUI()
    {
        //slider_text.text = "" + vaccinated.value;

        interval = interval - 1;
        if (interval == 0)
        {
            interval = 120;
            visitors = GetVisitors();
        }
        int num_infectious = 0;
        int num_healthy = 0;
        int num_new_infected = 0;
        int num_vaccinated = 0;
        int number_of_visitors = 0;
        foreach (VisitorAgents_2 visitor in visitors)
        {
            number_of_visitors += 1;
            switch (visitor.getStatus())
            {
                case "infected":
                    num_infectious += 1;
                    break;
                case "exposed":
                    num_new_infected += 1;
                    break;
                case "healthy":
                    num_healthy += 1;
                    break;
                case "vaccinated":
                    num_vaccinated += 1;
                    break;
            }
        }
        number_of_visits.text = "Number of visitors: " + number_of_visitors;
        infectious.text = "Number of infected: " + num_infectious;
        number_of_vaccinated.text = "Number of vaccinated: " + num_vaccinated;
        new_infections.text = "Number of newly infected: " + num_new_infected;

    }

    private void MoveCurrentObjectToMouse(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        int layerMask = LayerMask.GetMask("floor");
        RaycastHit hitInfo;
        if(Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask)){
            currentPlacableObject.transform.position = hitInfo.point + new Vector3(0, (currentPlacableObject.GetComponent<Collider>().bounds.size.y / 2), 0);
            currentPlacableObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
        }
        if(!flyCamera.getActive()){
            if(currentPlacableObject != null){
                Destroy(currentPlacableObject);
            }
        }
    }

    private void handleEsc(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            transform.GetChild(0).gameObject.active = !transform.GetChild(0).gameObject.active;
            flyCamera.setActive(transform.GetChild(0).gameObject.active);
        }
    }

    private void RotateFromMouseWheel(){
        mouseWheelRotation += Input.mouseScrollDelta.y;
        if(mouseWheelRotation > 36){
            mouseWheelRotation -= 36;
        }
        currentPlacableObject.transform.Rotate(Vector3.up, mouseWheelRotation * 10f);
    }

    private void LeftClickHandle(){
        if(Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.Delete) && !EventSystem.current.IsPointerOverGameObject()){

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = LayerMask.GetMask("UI");
            RaycastHit hitInfo_4;

            if(Physics.Raycast(ray, out hitInfo_4, Mathf.Infinity, layerMask)){
                return;
            }


            layerMask = LayerMask.GetMask("detector");
            RaycastHit hitInfo_3;

            if(Physics.Raycast(ray, out hitInfo_3, Mathf.Infinity, layerMask) && currentPlacableObject == null){
                ExpandMap(hitInfo_3);
                return;
            }

            layerMask = LayerMask.GetMask("booth");
            RaycastHit hitInfo_2;

            if(Physics.Raycast(ray, out hitInfo_2, Mathf.Infinity, layerMask) && currentPlacableObject == null){
                handleExistingBooth(hitInfo_2);
                return;
            }

            layerMask = LayerMask.GetMask("floor");
            RaycastHit hitInfo;

            if(Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask)){
                if(currentPlacableObject == null && selectedObjectPlacable != null){
                    newObjectPlacement(hitInfo);
                    return;
                }
                else if(currentPlacableObject != null){
                    selectedObjectPlacable.tag = "booth";
                    StoreScript script = selectedObjectPlacable.transform.GetComponent<StoreScript>();
                    if(upperBounds.text != ""){
                        Debug.Log(upperBounds.text);
                        script.end = int.Parse(upperBounds.text);
                    } else {
                        script.end = 120;
                    }
                    if(lowerBounds.text != ""){
                        script.start = int.Parse(lowerBounds.text);
                    } else {
                        script.start = script.end;
                    }
                    if(levelOfInterest.text != ""){
                        script.interest = int.Parse(levelOfInterest.text);
                    } else {
                        script.interest = 1;
                    }
                    currentPlacableObject = null;
                    return;
                }
                if(selectedObjectPlacable == null){
                    HandleBlockSettings(hitInfo);
                }
            }
        }
    }

    private void RightClickHandle(){
        if(Input.GetMouseButtonDown(1)){
            Destroy(currentPlacableObject);
        }
    }

    private void newObjectPlacement(RaycastHit hitInfo){
        currentPlacableObject = Instantiate(selectedObjectPlacable);
        selectedObjectPlacable.tag = "booth";
    }

    private void handleExistingBooth(RaycastHit hitInfo){
        selectedObject = hitInfo.collider.gameObject;
    }

    private void HandleBlockSettings(RaycastHit hitInfo){
        currentBlock = hitInfo.collider.gameObject;
    }

    private void HandleDel(){
        if(Input.GetKeyDown(KeyCode.Delete)){
            Destroy(selectedObject);
            selectedObject = null;
        }
    }

    private void HandleF(){
        if(Input.GetKeyDown("f")){
            flyCamera.setActive(!flyCamera.getActive());
        }
    }

    private void onEditClick(){
        transform.GetChild(0).gameObject.active = !transform.GetChild(0).gameObject.active;
    }

    private void ExpandMap(RaycastHit hitInfo){
        Vector3 pos = hitInfo.collider.transform.position;
        Transform detec_trans = hitInfo.collider.transform;
        Transform parent = hitInfo.transform.parent;
        Detector detec = hitInfo.collider.transform.GetComponent<Detector>();
        if(detec.get_neighbor() == null){
            // Instantiating new Building Block and getting its control script
            selectedBuildingBlockPlacable = Instantiate(selectedBuildingBlock);
            BuildingBlock current = selectedBuildingBlockPlacable
                .transform
                .GetComponent<BuildingBlock>();
            current.setUI(transform.gameObject);
            
            // If no length is chosen, it is set to 40
            float length;
            try{
                length = float.Parse(length_input.text);
            } catch {
                length = 40;
            }
            current.setLength(length);

            // Getting a detector from the new building block
            Transform connector = null;
            for (int i = 0; i < selectedBuildingBlockPlacable.transform.childCount; i++){
                if(selectedBuildingBlockPlacable.transform.GetChild(i).tag == "trigger"){
                    connector = selectedBuildingBlockPlacable.transform.GetChild(i);
                    break;
                }
            }

            //Applying the angles of the detector of the existing building block to the news
            selectedBuildingBlockPlacable.transform.eulerAngles = (detec.getAngle() + 180f * Vector3.up);

            Vector3 rel = connector.position - selectedBuildingBlockPlacable.transform.position;
            // Transfer the relative position of the detector to the centre from the existing building 
            // block to the new building block
            rel = Vector3.Scale(new Vector3(Mathf.Abs(rel.x), Mathf.Abs(rel.y), 
                Mathf.Abs(rel.z)), 
                (detec_trans.position - parent.position).normalized);

            //Applying the new position
            selectedBuildingBlockPlacable.transform.position = pos + rel;
        }
    }

    private void HandleLeftDel(){
        if(Input.GetKey(KeyCode.Delete) && Input.GetMouseButtonDown(0)){

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = LayerMask.GetMask("booth");
            RaycastHit hitInfo;
            if(Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask)){
                Destroy(hitInfo.collider.transform.gameObject);
                return;
            }

            layerMask = LayerMask.GetMask("floor");
            RaycastHit hitInfo_2;
            if(Physics.Raycast(ray, out hitInfo_2, Mathf.Infinity, layerMask)){
                Transform parent = hitInfo_2.collider.transform.parent;
                BuildingBlock block = parent.GetComponent<BuildingBlock>();
                foreach(Transform booth in block.getExhibitors()){
                    Destroy(booth.gameObject);
                }
                Destroy(parent.gameObject);
                return;
            }
            // Entferne einzelne Booths oder Hallen, je nachdem was vom Ray getroffen wird
        }
    }

    private void onStartClick()
    {
        if (!started)
        {
            transform.GetChild(0).gameObject.active = !transform.GetChild(0).gameObject.active;
            flyCamera.setActive(transform.GetChild(0).gameObject.active);

            int num_visitors = get_number_visitors();

            float spawn_interval = get_spawn_interval();

            float probability_infec = get_probability_infec();

            float share_vaccinated = get_vaccinated();

            spawns = GetSpawns();

            foreach (SpawnScript spawn in spawns)
            {
                spawn.start_spawn(num_visitors, spawn_interval, share_vaccinated, probability_infec);
            }
            started = true;
        }
    }

    private void onStopClick()
    {
        if (started)
        {
            foreach(SpawnScript spawn in spawns)
            {
                spawn.stop_spawn();
            }
            GameObject[] objects = GameObject.FindGameObjectsWithTag("visitor");
            foreach (GameObject obj in objects)
            {
                Destroy(obj);
            }
            started = false;
        }
    }

    private List<VisitorAgents_2> GetVisitors()
    {
        List<VisitorAgents_2> current_visitors = new List<VisitorAgents_2>();
        GameObject[] objects = GameObject.FindGameObjectsWithTag("visitor");
        foreach (GameObject obj in objects)
        {
            VisitorAgents_2 new_visitor = obj.transform.parent.GetComponent<VisitorAgents_2>();
            current_visitors.Add(new_visitor);
        }
        return current_visitors;
    }

    private List<SpawnScript> GetSpawns()
    {
        List<SpawnScript> current_spawns = new List<SpawnScript>();
        GameObject[] spawn_Objects = GameObject.FindGameObjectsWithTag("spawnarea");
        foreach (GameObject obj in spawn_Objects)
        {
            SpawnScript spawn = obj.transform.GetComponent<SpawnScript>();
            current_spawns.Add(spawn);        }
        return current_spawns;
    }

    private float get_spawn_interval()
    {
        float spawn_interval;
        if (spawn_interval_input.text == "")
        {
            spawn_interval = 0.5f;
        }
        else if (float.Parse(spawn_interval_input.text) < 0 || float.Parse(spawn_interval_input.text) > 10)
        {
            spawn_interval = 0.5f;
        }
        else
        {
            spawn_interval = float.Parse(spawn_interval_input.text);
        }
        return spawn_interval;
    }

    private int get_number_visitors()
    {
        int num_visitors;
        if (visitors_input.text == "")
        {
            num_visitors = 10;
        }
        else if (int.Parse(visitors_input.text) > 400 || int.Parse(visitors_input.text) < 0)
        {
            num_visitors = 10;
        }
        else
        {
            num_visitors = int.Parse(visitors_input.text);
        }
        return num_visitors;
    }

    private float get_probability_infec()
    {
        int inzidenz;
        if(inzidenz_input.text == "")
        {
            inzidenz = 10;
        } 
        else if(int.Parse(inzidenz_input.text) > 100000 || int.Parse(inzidenz_input.text) < 0)
        {
            inzidenz = 10;
        }
        else
        {
            inzidenz = int.Parse(inzidenz_input.text);
        }
        float prob = (float)inzidenz / 100000f;
        return prob;
    }

    private void onMaskValue(Dropdown change){
        switch(change.value){
            case 0:
                maskText = "None";
                break;
            case 1:
                maskText = "Surgical";
                break;
            case 2:
                maskText = "N95";
                break;
        }
    }


    private float get_vaccinated()
    {
        float share = vaccinated.value;
        return share;
    }

    private void onBoothClick()
    {
        selectedObjectPlacable = booth;
    }

    private void onFoodClick()
    {
        selectedObjectPlacable = food;
    }

    private void onHallwayClick()
    {
        selectedBuildingBlock = hallway;
    }

    private void onHallClick()
    {
        selectedBuildingBlock = hall;
    }

    private void onSectionClick(){
        selectedBuildingBlock = innsersection;
    }

    private void onSpawnClick(){
        selectedBuildingBlock = spawn;
    }

    private void onExitClick(){
        selectedBuildingBlock = exit;
    }

    private void onBooth1Click(){
        selectedObjectPlacable = booth1;
    }

    private void onBooth2Click(){
        selectedObjectPlacable = booth2;
    }
    
    private void onBooth3Click(){
        selectedObjectPlacable = booth3;
    }
    
    private void onBooth4Click(){
        selectedObjectPlacable = booth4;
    }

    public string getMask(){
        return maskText;
    }
}