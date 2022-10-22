using Godot;
using System;

/*	obousmerny Komunikacni kanal pro predavani zprav i argumentu z Csharp do GDscriptu a naopak
	- vola funkci message_update v communicationGDObjectu = pouzivame pro okamzite predani zpravy
	- ve chvili kdy v communicationGDObjectu
*/
public partial class MessageObject: Godot.Object
{
	// s kym si predavame zpravy
	private Node nodeSelf = null;
	private Node communicationGDObject = null;

	// hlavni message - udava co chceme
	private string message = "msg_nothing";

    // ruzne typy dat - pro nahrazeni argumentu v call
    private bool boolData = false;
    private int intData = 0;
	private float floatData = 0.0f;
    private string stringData = "no_string_data";
	private Vector2 vector2Data = Vector2.Zero;
	private Vector3 vector3Data = Vector3.Zero;
	private Node nodeData = null;

	// Constructor, kde musime pridat s jakym nodem budeme komunikovat
	public MessageObject(Node newNodeSelf,Node newCommunicationGDObject)
	{
		if (newCommunicationGDObject == null)
			GD.Print("pri konstrukci MessageObject se parametr newCommnucationGDObject == null");
		else
			communicationGDObject = newCommunicationGDObject;

		if (newNodeSelf == null)
			GD.Print("pri konstrukci MessageObject se parametre newNodeSelf == null");
		else
			nodeSelf = newNodeSelf;

		ResetAllData();
	}

    private void SetMessage(string newMessage)
	{
		message = newMessage;
    }

	public void SendMessageToGDNow(string newMessage)
	{
        // Posle zpravu z CS do GD objektu
		SetMessage(newMessage);
        communicationGDObject.Call("message_update");
    }

	public void SendMessageToCSNow(string newMessage)
	{
        // Posle zpravu z GD do CS objektu
		SetMessage(newMessage);
		nodeSelf.Call("message_update");
	}

	public void SetMessageNothing()
	{
		message = "msg_nothing";
	}

	public string GetMessage()
	{
		return message;
	}

	public void ResetAllData()
	{
		stringData = "msg_nothing";
		intData = 0;
		nodeData = null;
	}

    // GET LOCAL SAVED DATA
	// BOOL DATA
	public void SetBoolData(bool newBoolData) { boolData = newBoolData; }
	public bool GetBoolData() { return boolData; }

    // INT DATA
    public void SetIntData(int newIntData) { intData = newIntData; }
	public int GetIntData() { return intData; }

    // FLOAT
    public void SetFloatData(float newFloatData) { floatData = newFloatData; }
    public float GetFloatData() { return floatData; }

    // STRING DATA
    public void SetStringData(string newStringData) {stringData = newStringData;}
    public string GetStringData() { return stringData; }

    // VECTOR 2 DATA
    public void SetVector2Data(Vector2 newVector2Data) { vector2Data = newVector2Data; }
    public Vector2 GetVector2Data() { return vector2Data; }

    // VECTOR 3 DATA
    public void SetVector3Data(Vector3 newVector3Data) { vector3Data = newVector3Data; }
    public Vector3 GetVector3Data() { return vector3Data; }

    // NODE DATA
    public void SetNodeData(Node newData) {nodeData = newData;}
    public Node GetNodeData() { return nodeData; }

    // FINAL GET PARAMETERS (instead return call)
    // FOR BIT EASE (CS GET FROM GD) PARAMETER
    public bool LoadBoolDataFromGDNow(string newMessage)
    {
        SendMessageToGDNow(newMessage);
        bool tempData = GetBoolData();
        return tempData;
    }

    public int LoadIntDataFromGDNow(string newMessage)
    {
        SendMessageToGDNow(newMessage);
        int tempData = GetIntData();
        return tempData;
    }

    public float LoadFloatDataFromGDNow(string newMessage)
    {
        SendMessageToGDNow(newMessage);
        float tempData = GetFloatData();
        return tempData;
    }

    public string LoadStringDataFromGDNow(string newMessage)
    {
        SendMessageToGDNow(newMessage);
        string tempData = GetStringData();
        return tempData;
    }

    public Vector2 LoadVector2DataFromGDNow(string newMessage)
    {
        SendMessageToGDNow(newMessage);
        Vector2 tempData = GetVector2Data();
        return tempData;
    }

    public Vector3 LoadVector3DataFromGDNow(string newMessage)
    {
        SendMessageToGDNow(newMessage);
        Vector3 tempData = GetVector3Data();
        return tempData;
    }

    public Node LoadNodeDataFromGDNow(string newMessage)
    {
        SendMessageToGDNow(newMessage);
        Node tempData = GetNodeData();
        return tempData;
    }

    // FOR BIT EASE (GD GET FROM CS) PARAMETER

    public bool LoadBoolDataFromCSNow(string newMessage)
    {
        SendMessageToCSNow(newMessage);
        bool tempData = GetBoolData();
        return tempData;
    }

    public int LoadIntDataFromCSNow(string newMessage)
    {
        SendMessageToCSNow(newMessage);
        int tempData = GetIntData();
        return tempData;
    }

    public float LoadFloatDataFromCSNow(string newMessage)
    {
        SendMessageToCSNow(newMessage);
        float tempData = GetFloatData();
        return tempData;
    }

    public string LoadStringDataFromCSNow(string newMessage)
    {
        SendMessageToCSNow(newMessage);
        string tempData = GetStringData();
        return tempData;
    }

    public Vector2 LoadVector2DataFromCSNow(string newMessage)
    {
        SendMessageToCSNow(newMessage);
        Vector2 tempData = GetVector2Data();
        return tempData;
    }

    public Vector3 LoadVector3DataFromCSNow(string newMessage)
    {
        SendMessageToCSNow(newMessage);
        Vector3 tempData = GetVector3Data();
        return tempData;
    }

    public Node LoadNodeDataFromCSNow(string newMessage)
    {
        SendMessageToCSNow(newMessage);
        Node tempData = GetNodeData();
        return tempData;
    }
}
