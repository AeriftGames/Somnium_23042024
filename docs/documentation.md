# Documentation
Technická dokumentace k projektu **Somnium Prototype**

## Seznam kapitol
- [Documentation](#documentation)
  - [Seznam kapitol](#seznam-kapitol)
- [Standardy projektu](#standardy-projektu)
  - [Souborová struktura projektu](#souborov-struktura-projektu)
  - [Jmenová konvence](#jmenov-konvence)
- [Herní mechanismy a jejich detailní popisy](#hern-mechanismy-a-jejich-detailn-popisy)
  - [MessageObject](#messageobject)
  - [FPSCharacter](#fpscharacter)
  - [GameMaster](#gamemaster)
  - [item_pickup](#item_pickup)
- [Groups](#groups)
  - [template_group](#template_group)
- [Collision layers](#collision-layers)
- [Autoload](#autoload)
  - [logging](#logging)
  - [CustomSettings](#customsettings)
  - [LastSingleton](#lastsingleton)
- [Databases](#databases)
  - [items](#items)

*Pro rychlé vygenerování kapitol lze použít [https://ecotrust-canada.github.io/markdown-toc/](https://ecotrust-canada.github.io/markdown-toc/)*
*Ještě další generator, kde je nastavit úroveň. Současné kapitoly jsou udělané na úroveň **3**. [https://luciopaiva.com/markdown-toc/](https://luciopaiva.com/markdown-toc/)

# Standardy projektu
Tato kapitola obsahuje dohodnoté standardy, které zajišťují jednoduchou přehlednost pro všechny členy projektu

## Souborová struktura projektu
**TODO** Zde bude popsána složková struktura projektu.

## Jmenová konvence
**TODO** Tato kapitola obsahuje menší kapitoly jednotlivých jmennových konvencí.

### Pojmenování složek
**TODO**

### Pojmenování scén a scriptů
**TODO**

### Pojmenování složek
**TODO**

### Pojmenování nodes v rámci scén
**TODO**

# Herní mechanismy a jejich detailní popisy
Tato kapitola obsahuje všechny důležité herní mechanismy

## **MessageObject**
Jedná se o classu která ulehčuje obejít nepříjemné chování Godot 4 (které nejspíš bude časem s novými verzemi Godot enginu vyřešeno) Aktuálně problém je v tom, že nemůžeme předat skrze argumenty ve funci jakákoliv data z **CSharp scriptu** do **GD scriptu** a naopak a to ani vracet funkcí nějaký datový typ. Godot totiž řeší veškeré datové konverze argumentů mezi jednotlivými ```Godot.Object``` přes svůj typ ```Variant```. Typ Variant umí pracovat s velkým počtem používaných typů a dělá tedy jejich konverzi (například int nebo string na Variant) Když se tedy například pokoušíme přenést skrze argumenty funkce data, začne se plnit neznámý buffer v cashe paměti (kterou využívá Variant) a to nejšpíš proto, že se po přenosu neuvolní data z této části paměti (neznámý buffer). Program se po chvílí přehlcuje a i engine přestává správně fungovat, **nastávají úniky paměti** a začíná padat jak náš program, tak samotný engine. Proto je tedy MessageObject, který slouží jako jiná cesta komunikace, než by jsme v tomto případě jinak nativně používali.

### **Ukázka základního použití MessageObject**
Dejme tomu že máme takovou scénu v Godotu a chceme komunikovat mezi těmito nodes i s možností předávat si různá data:
- WorldScene
  - **NodeA_GD** (Node3D obsahující GD script)
  - **NodeB_CS** (Node3D obsahující CS script)

**NodeA_GD** (GD script)
```python
extends Node3D

# objekt s kterym chceme komunikovat a ma v sobe msgObject
var NodeB

func _ready():
    NodeB = get_node("/root/WorldScene/NodeB_CS")
    # posle zpravu ktera z kontextu chce vratit string nejakeho jmena
    NodeB.msgObject.SendMessageToCSNow("msg_get_yourname")
    print(NodeB.msgObject.GetStringData())

# toto je funkce volana pouze z protejsiho komunikacniho objektu CS (neco jako hej mas zpravu, ted si ji precti)
func message_update():
	# cteme a filtrujeme prichozi povahu message z NodeB_CS
	var msg:String = NodeB.msgObject.GetMessage()
	if(msg == "msg_write_text"):
		print(NodeB.msgObject.GetStringData())
```

**NodeB_CS** (CS script)
```csharp
using Godot;
using System;

public partial class NodeB_CS : Node3D
{
  // nas MessageObject
  public MessageObject msgObject;

  // objekt s kterym chceme komunikovat
  Node3D NodeA;

	public override void _Ready()
	{
      // vytvoreni noveho MessageObjectu
      msgObject = new MessageObject(this,GetNode<Node3D>("/root/WorldScene/NodeA_GD"));
      
      // Nacteni nodu s kterym chceme komunikovat
      NodeA = GetNode<Node3D>("/root/WorldScene/NodeA_GD");

      // nastavime data(jako argument) a posilame NodeA_GD zpravu (msg/kontext)
      msgObject.SetStringData("hello");
      msgObject.SendMessageToGDNow("msg_write_text");
      // SendMessageToGDNow spusti v protejsim objektu s kterym komunikujeme funkci message_update
	}

	// toto je funkce volana pouze z protejsiho komunikacniho objektu GD (neco jako hej mas zpravu, ted si ji precti)
	public void message_update()
	{
		// cteme a filtrujeme prichozi povahu message
		string msg = msgObject.GetMessage();
		switch (msg)
		{
            case "msg_get_yourname":
                {
                    msgObject.SetStringData("Michael");
                    break;
                }
        }
	}
}
```

### **Vyvětlení kroků v kódu:**
Vytvoření ```msgObject``` musí být vždy na straně CS scriptu a kvůli domluvě by se měl tento objekt vždy jmenovat msgObject

**CS SCRIPT**
```csharp
// Vytvorime novy message object ktery bude komunikovat s jinym objektem s GD scriptem
// Prvni parametr je odkaz na tuhle instanci objektu
// Druhy parametr je instance objektu s kterym chceme komunikovat
msgObject = new MessageObject(this,GetNode<Node3D>("/root/WorldScene/NodeA_GD"));
```
Pošleme zprávu a text jako argument **(první se musi nastavit data co chceme předat a až poté poslat zprávu)**
```csharp
// nastavime data(jako argument) a posilame NodeA_GD zpravu (msg/kontext)
msgObject.SetStringData("hello");
msgObject.SendMessageToGDNow("msg_write_text");
```
v tu chvíli kde končí předchozí kód ```SendMessageToGDNow``` se přesune kód do GD scriptu a zavolá v něm ```message_update()```

**GD SCRIPT**

teď se spustí message_update, kde si objekt přečte zprávy
```python
# toto je funkce volana pouze z protejsiho komunikacniho objektu CS (neco jako hej mas zpravu, ted si ji precti)
func message_update():
	# cteme a filtrujeme prichozi povahu message z NodeB_CS
	var msg:String = NodeB.msgObject.GetMessage()
	if(msg == "msg_write_text"):
		print(NodeB.msgObject.GetStringData())
```
přečteme si zprávu a pokud se shoduje, tak následným kódem zareagujeme (jelikož jsme v **CS Objektu** před odesláním zprávy nastavili ```SetStringData("hello")``` **viz výše**.) můžeme si poslední uložená data lehce převzít, je tam uložen string ```"hello"```, který pomocí printu vypíšeme
```python
if(msg == "msg_write_text"):
    print(NodeB.msgObject.GetStringData())
```
**Veškerá komunikace funguje i obráceně** a je zanesena v ukázkovém kódu výše

**GD SCRIPT** 

Pošle zprávu ```SendMessageToCSNow()``` do **CS Scriptu** a bude chít přijímat data string
```python
func _ready():
    NodeB = get_node("/root/WorldScene/NodeB_CS")
    # posle zpravu druhemu objektu a ktera z kontextu chce vratit string nejakeho jmena
    NodeB.msgObject.SendMessageToCSNow("msg_get_yourname")
    print(NodeB.msgObject.GetStringData())
```
**CS SCRIPT** 

Hned poté co poslal zprávu **GD Script** skrze ```SendMessageToCSNow()``` se spustí v **CS Scriptu** funkci ```message_update()``` kde si opět si přečteme zprávy. Pokud zpráva souhlasí, provedeme reakci zprávu, naplníme stringData textem nejakeho jmena.
```csharp
// toto je funkce volana pouze z protejsiho komunikacniho objektu GD (neco jako hej mas zpravu, ted si ji precti)
	public void message_update()
	{
		// cteme a filtrujeme prichozi povahu message
		string msg = msgObject.GetMessage();
		switch (msg)
		{
            case "msg_get_yourname":
                {
                    msgObject.SetStringData("Michael");
                    break;
                }
        }
	}
```
V tu chvíli kdy projede message_update se vrátí kód zpátky do **GD Scriptu** hned tam kde skončil po ```SendMessageToCSNow()```, takže následuje:
```python
print(NodeB.msgObject.GetStringData())
```
kde si vezmeme právě uloženou hodnotu string Michael.

### **Volatelné funkce msgObject**
| Funkce      | Argumenty | Popis |
| ----------- | ----------- | -----------|
| ```MessageObject()```        | ```Node``` ownerObject, ```Node``` communicationObject, ```(optional)bool``` multiCommunication          | Konstruktor při vytváření objektu MessageObject,**(optional bool)** jen jestli budeme chtít komunikovat s více GD Scripty naráz|
| ```SendMessageToGDNow()```   | ```string```                                        | Pošle zprávu z CS do GD objektu|
| ```SendMessageToCSNow()```   | ```string```                                        | Pošle zprávu z GD do CS objektu|
| ```GetMessage()```           |                                                                | Vrátí aktualní zprávu          |
| ```ResetAllData()```         |                                                                | Zresetuje **včechna** data pro přenos informací|
| ```GetIsMultiCommunication()```         |                                                                | vrací **true**, pokud je při konstrukci počítáno s komunikací s vícero GDScripty naráz|
| ```SendMessageToGDNow_ToObject()```         | ```string``` message, ```Node``` newCommunicationGDObject        | Pošle zprávu z CS do GD objektu, který nově přiřadíme (jen pokud v konstruktoru MessageObject je argument **multiCommunication** = true)|
| ```SetBoolData()```          | ```bool```                                         | Nastaví nová **bool** data pro přenos informace|
| ```GetBoolData()```          |                                                                | Vrací aktuální **bool** data pro přenos informace|
| ```SetIntData()```           | ```int```                                           | Nastaví nová **int** data pro přenos informace|
| ```GetIntData()```           |                                                                | Vrací aktuální **int** data pro přenos informace|
| ```SetFloatData()```         | ```float```                                       | Nastaví nová **float** data pro přenos informace|
| ```GetFloatData()```         |                                                                | Vrací aktuální **float** data pro přenos informace|
| ```SetStringData()```         | ```string```                                       | Nastaví nová **string** data pro přenos informace|
| ```GetStringData()```         |                                                                | Vrací aktuální **string** data pro přenos informace|
| ```SetVector2Data()```         | ```Vector2```                                       | Nastaví nová **Vector2** data pro přenos informace|
| ```GetVector2Data()```         |                                                                | Vrací aktuální **Vector2** data pro přenos informace|
| ```SetVector3Data()```         | ```Vector3```                                       | Nastaví nová **Vector3** data pro přenos informace|
| ```GetVector3Data()```         |                                                                | Vrací aktuální **Vector3** data pro přenos informace|
| ```SetNodeData()```         | ```Node```                                       | Nastaví nová **Node** data pro přenos informace|
| ```GetNodeData()```         |                                                                | Vrací aktuální **Node** data pro přenos informace|
| ```SetNodeData()```         | ```Node```                                       | Nastaví nová **Node** data pro přenos informace|
| ```LoadBoolDataFromGDNow()```         | ```string``` messge                             | Stejný jako  ```SendMessageToGDNow()``` + ihned vrací očekávaná **bool** data|
| ```LoadIntDataFromGDNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToGDNow()``` + ihned vrací očekávaná **int** data|
| ```LoadFloatDataFromGDNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToGDNow()``` + ihned vrací očekávaná **float** data|
| ```LoadStringDataFromGDNow()```         | ```string``` message                             | Stejný jako ```SendMessageToGDNow()``` + ihned vrací očekávaná **string** data|
| ```LoadVector2DataFromGDNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToGDNow()``` + ihned vrací očekávaná **Vector2** data|
| ```LoadVector3DataFromGDNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToGDNow()``` + ihned vrací očekávaná **Vector3** data|
| ```LoadNodeDataFromGDNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToGDNow()``` + ihned vrací očekávaná **Node** data|
| ```LoadBoolDataFromCSNow()```         | ```string``` messge                             | Stejný jako  ```SendMessageToCSNow()``` + ihned vrací očekávaná **bool** data|
| ```LoadIntDataFromCSNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToCSNow()``` + ihned vrací očekávaná **int** data|
| ```LoadFloatDataFromCSNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToCSNow()``` + ihned vrací očekávaná **float** data|
| ```LoadStringDataFromCSNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToCSNow()``` + ihned vrací očekávaná **string** data|
| ```LoadVector2DataFromCSNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToCSNow()``` + ihned vrací očekávaná **Vector2** data|
| ```LoadVector3DataFromCSNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToCSNow()``` + ihned vrací očekávaná **Vector3** data|
| ```LoadNodeDataFromCSNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToCSNow()``` + ihned vrací očekávaná **Node** data|

**Tipy k funkcím**
- Pokud použijeme v konstruktoru MessageObject ```(optional)bool``` argument **mutliCommunication** = true, nemůžeme pak na tomhle MessageObjectu využívat zkrácených funkcí pro vracení dat jako je třeba: ```LoadVector3DataFromGDNow()```, můžeme používat pouze ```SendMessageToGDNow_ToObject()```, je to prevence nechtěné komunikaci se špatným communicationObject.

## **FPSCharacter**
Do budoucna bude přidaný popis vlastností a funkcí FPSCharactera, prozatím zde budou alespoň **z venku volatelné funkce**


### **Volatelné funkce FPSCharacter**
| Typ | Funkce | Popis funkce |
|---|--------|-----------|
|void|```SetInputEnable```( **bool** newEnable )                                                  |**Zapne/Vypne** veškeré ovládání charactera|
|void|```SetMouseVisible```( **bool** newMouseVisible )                                           |**Zapne/Vypne** viditelnost kurzoru myši   |
|bool|```IsInputEnable()```                                                                       |**Vrací true/false**, podle aktuálního stavu|
|void|```DisableInputsAndCameraMoveLookTarget```( **Vector3** targetPos, **Vector3** targetLook ) |Vypne ovládání charactera, nechá hráčovo kameru dolerpovat na **targetPos** a dolerpuje look kamery na **targetLook**|
|void|```EnableInputsAndCameraToNormal()```                                                       |Nechá hráčovo kameru dolerpovat do výchozího stavu a poté zapne ovládání charactera|

## **GameMaster**
Jedná se o **Singleton**, spuštěný z autoloadu. Lze využít jako takový univerzální prostředník v rámci herní instance i enginu.
- Lze se jednoduše (do budoucna bezpečně) dostat na aktuální **instanci** hráče **FPSCharacter**
- Lze se jednoduše dostat na instanci **BasicHud**, **DebugHud**
- Lze se z **C#** strany dovolávat na ostatní **GDScript** objekty v autoloadu (například **Logging**, **CustomSettings**)
- Do budoucna se počítá s rozšíření mnoha užitečných funkcí
- z **C#** přistupujeme k **Instanci** GameMastera přes ```GameMaster.GM``` což je statická instance našeho GameMastera v Autoloadu
- z GDScriptu přistupujeme k **Instanci** GameMastera přes **Node** v autoloadu se stejným názvem **GameMaster**, tudíž ```get_node("/root/GameMaster")``` a poté přístupu k **Instanci** přes ```.GM```
- obsahuje LogSystem **Log** pro pro práci **(GD) Logging**

### **Volatelné funkce GameMaster**

Volatelné skrze ```GameMaster.GM.```

| Typ | Funkce | Popis funkce |
|---|--------|-----------|
|void|```SetFPSCharacter```( **FPSCharacter_basicMoving** newFPSCharacter )| Nastaví aktuální instanci **FPSCharacter** (provádíme ze strany FPSCharacter._Ready())|
|FPSCharacter_BasicMoving|```GetFPSCharacter()```                          |**Vrátí** Instanci **FPSCharacter** (hráče) pro volání jeho funkcí|
|void|```SetDebugHud```( **DebugHud** newDebugHud )|Nastaví aktuální instanci **DebugHud** (provádíme ze strany DebugHud._Ready())|
|DebugHud|```GetDebugHud()```|Vrátí Instanci **DebugHud** pro volání jeho funkcí|

## item_pickup
Item_pickup je **core system** nacházející se v `core_systems\item_pickup`. Item_pickup slouží jako základ pro všechny
itemy, které mají být zvednutelné (pick up) hráčem.

### Popis
Systém je dělán tak, aby jej stačilo jen připojit k itemu, u kterého chceme povolit pick up. A následně nastavit
proměnné pro doladění potřebného chování (popsáno v kapitole) [Jak použít](#Jak-použít). Většina proměnných má
defaultní hodnotu, ale některé no. Pro správné fungování by mělo být všech not null.

### Jak použít
Použití item_pickup je jednoduché. Stačí provést následující kroky:
1. Vytvořit scénu, v které chceme logiku picku provést. Může jít například o scénu vytvořenou rovnou z *.glb* souboru
pomocí `New Inherited scene` (pravé tlačítko myši na *.glb* soubor)
2. K parenté této scény přiřadíme script nacházející se v `core_systems\item_pickup\item_pickup.gd`
3. Po připojení odoznačíme parent a znovu označíme. Tím, se aktualizuje inspector, který bude obsahovat nové proměnné.
Viz. obrázek:
![Ukázka](https://github.com/AeriftGames/Somnium/blob/develop/docs/img/item_pickup_01.PNG)
4. Zde **musíme** nastavit všechny proměnné. Bez nich nebude logika fungovat správně.

| Variable              | Popis                                                                                                        | Typ         | Default value |
|-----------------------|--------------------------------------------------------------------------------------------------------------|-------------|---------------|
| item_interaction_name | Jméno typu akce, které se zobrazí hráči po najetí na item. Mělo by začínat velkým písmenem.                  | String      | Pickup        |
| item_name             | Jméno itemu, které se zobrazí hráči po najetí na item. Mělo by začínat malým písmenem.                       | String      | item          |
| use_node              | Node s připojeným scriptem, který **musí** obsahovat `use()` funkci. Tato funkce je zavolána po použití itemu. | Node        | *null*        |
| pickup_speed          | Rychlost itemu, kterou letí směrem k hráči.                                                                  | float       | 0.2           |
| pickup_height         | Výška, do které letí item (Vypočítáno z pozice playera). 0 je zem, na které hráč stojí.                      | float       | 0.8           |
| hide_speed            | Rychlost, při které se item schová (aby simuloval osebrání objektu)                                          | float       | 0.1           |
| sfx                   | Zvukový efekt, který se přehraje při sebrání itemu. Po dohrání zvuku dojde k `queue_free()`.                 | AudioStream | *null*        |

5. Následně vytvoříme nový node, který bude obsahovat kód s logikou, která se provede po sebrání itemu. Tento node je 
je potřeba vybrat v proměnné **use_node**
6. Use node musí obsahovat funkcí use(), která je zavoláná po sebrání itemu.
7. Jako poslední připojíme pod nejvyšší node instanci core systému **interactive_object**, který se nachází v
`core_systems\interactive_system`

## item_use
item_use je **core_systém** nacházející se v `core_systems\item_use`. Slouží jako základ pro logiku aktivace/použití
určité věci hráčem.

### Popis
Systém je dělán tak, aby jej stačilo jen připojit k itemu, u kterého chceme povolit pick up. A následně nastavit
proměnné pro doladění potřebného chování (popsáno v kapitole) [Jak použít](#Jak-použít). Většina proměnných má
defaultní hodnotu, ale některé no. Pro správné fungování by mělo být všech not null.

### Jak použít
Použití item_use je jednoduché. Stačí provést následující kroky:
1. Vytvořit scénu, v které chceme logiku use provést. Může jít například o scénu vytvořenou rovnou z *.glb* souboru
pomocí `New Inherited scene` (pravé tlačítko myši na *.glb* soubor)
2. K parenté této scény přiřadíme script nacházející se v `core_systems\item_use\item_use.gd`
3. Po připojení odoznačíme parent a znovu označíme. Tím, se aktualizuje inspector, který bude obsahovat nové proměnné.
Viz. obrázek:
![Ukázka](https://github.com/AeriftGames/Somnium/blob/develop/docs/img/item_pickup_01.PNG)
4. Zde **musíme** nastavit všechny proměnné. Bez nich nebude logika fungovat správně.

| Name                  | Description                                                                                                 | Type        | Default value |
|-----------------------|-------------------------------------------------------------------------------------------------------------|-------------|---------------|
| item_interaction_name | Jméno typu akce, které se zobrazí hráči po najetí na item. Mělo by začínat velkým písmenem.                 | String      | Open          |
| item_name             | Jméno itemu, které se zobrazí hčáti po najetí na item. Mělo by začínat malým písmenem.                      | String      | drawer        |
| use_node              | Node s připojeným scriptem, který **musí** obsahovat use() funkci. Tato funkce je zavolána po použití itemu | Node        | null          |
| sfx                   | Zvukový efekt, který se přehraje při aktivaci itemu.                                                        | AudioStream | null          |

5. Následně vytvoříme nový node, který bude obsahovat kód s logikou, která se provede po sebrání itemu. Tento node je je potřeba vybrat v proměnné use_node
6. Use node musí obsahovat funkcí use(), která je zavoláná po sebrání itemu.
7. Jako poslední připojíme pod nejvyšší node instanci core systému **interactive_object**, který se nachází v
`core_systems\interactive_system`

# Groups
Tato kapitola popisuje veškeré groups využité v projektu

## template_group
Toto je ukázková group. Tuto skupinu obsahují všechny objekty, které budou být moci interaktovány s hráčem.

# Collision layers
Tato kapitola popisuje veškeré kolizní vrstvy v projektu.

### 1. vrstva: **worldphysics_layer**
Tato vrstva je defaultní a řeší základní kolizi 3D světa. V této vrstvě se nachází veškeré překážky, zdi, objekty se kterými chceme aby kolidoval hráč, ale také s nimiž by měl počítat třeba navmesh nepřátel.

### 2. vrstva: **interactive_layer**
Tato vrstva je zamýšlena výhradně pro detekci raycastem FPSCharacter na ```interactive_object``` (interakce hráče) a tato vrstva není detekována ani v kolizi s žádnou jinou kolizní vrstvou. Lze ji hitnout pouze raycastem a nemělo by v této vrstvě být nic jiného, ani by ji neměl nikdo detekovat.

# Autoload
Tato kapitola popisuje všechny autoload.

## logging
Logging by měl být vždy na prvním místě v pořadí Autoloadů. Je to z důvodu toho, aby byl dostupný pro případné autoloady, které by chtěli logovat.
Detailní popis funkčnosti loggingu je popsán [v této kapitole](#logging).

## CustomSettings
Jedná se o autoload, který do svých proměnných ukládá veškeré hodnoty z [configu](#config). Lze si tedy šáhnout na jakoukoliv hodnotu z configu například pomocí:
```python
if CustomSettings.debug_oalar == true:
    do_something()
```

## LastSingleton
Jedná se o pomocný Autoload pro Logging. Jediné co tento autoload dělá je, že nastavít hodnotu 
```
Logging.autoload_complete = true
```
Pomocí této hodnoty logging pozná, že již proběhl veškerý loading. Po načtení se LastSingleton sám uvolní.

# Databases
Tato kapitola popisuje jednotlivé databáze využíté v projektu

## items
items.json je databáze itemů určená k TODO
