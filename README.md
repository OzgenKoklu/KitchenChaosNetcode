# CodeMonkey2023-Overcooked-Clone Online with netcode / Lobby / relay

/WORK IN PROGRESS/
 This is the multiplayer complete tutorial covered by Code Monkey in this video: https://www.youtube.com/watch?v=7glCsF9fv3s
This game project started as a Singleplayer only game, then netcode support was added later on, I've also uploaded the complete sinpleplayer repo in github: https://github.com/OzgenKoklu/KitchenChaosSingleplayer
In this readme, I only mention the Netcode/Lobby/Relay related features and coding, the repository for Singleplayer project include the details on main game mechanics.

Disclaimer: All assets were borrowed from Code Monkey and are not used for commercial use, this is a learning project and I've built this on my own under instructions by Code Monkey. 
This built wont run on your computer as is unless linked to Unity Services, to link go to Edit > Project Settings > Services and link to a project in which Lobby and Relay services are enabled.

ABOUT THE GAME: This is a small scale casual game that is highly inspired by OverCooked, where you have to deliver specific orders in a time limit. 
It is a complete package with scene cycle, sound and animation, options menu, savable settings and key bindings with added multiplayer! 

/IMAGES AND VIDEO TO BE ADDED/\
Screenshots: \
-main menu ss\
-lobby list ss\
-lobby scene ss\
-Game play ss

Videos & Gifs: 
-Lobby color change gif\
-Gameplay gif \

-YT Link: 

TLDR: In Short: What I've learned from this project:\
-Implementing Netcode For Gameobject, Lobby & Relay by Unity to an already existing singleplayer game\
-Basic understanding of multiplayer game logic: Server/Client authoritative design meaning, RPC's, basic solutions for bad connection\
-Making a fully working game scene flow using netcode for gameObjects, then using lobby and relay services to connect online\

On my decision to seperate the project in two repositories:
Short answer: The source code changed too much. And since I'm much new to multiplayer development, I'm not much familiar with structures and design choiches that were implemented in this current state.\
Before this project, I havent done any multiplayer game project, I've heard of Photon Network and I think it still is a very popular option to make a multiplayer game.\
The architectural needs of a multiplayer game is completely different and the programmer has to decide whether the game would be server authoritative or client authoritative and this alone changes the structure of the project. \
In singleplayer games, especially when you are in the very beggining in your developer journey, you only deal with whats on the screen, you make things change in behaviour to make the game play, however, \
the entire communication has to be tought out clearly for multiplayer development. Theres some essence of backend development in this sense where requests should be dealt with in particular ways. \
This project helped me further understand the C# and OOP concepts, I've understood while building it, but yet I feel unfamiliar still.\

More about whats covered in this project for those who have more time: \
The project is a course project by Code Monkey and has its curriculum in this link: https://unitycodemonkey.com/kitchenchaosmultiplayercourse.php\
However, I want to re-visit my own commits and make my own list of what I've learned:

What I've learned: \
-ServerRPC/ClientRPC's and how to set up a basic multiplayer architecture \
-Server Authoritative/Client Authoritative meaning and usage\

***Translate this
1) Multi ve single'ın en büyük farkı client ve serverın cihazlarında çalışacak kodların farklı olması gerektiğini hesaba katmamız gerekmesi.\
Bu hem bazı şeyleri valide etmek hem de bazı şeylerin senkron çalışmasını garantilemek için baştan düşünmemiz gereken engellerle geliyor. DeliveryManager'ın sadece serverda çalışması ve clientlara \
sonuç raporu iletmesi ve clientların bu sonuca göre ekranda bir şeyler göstermesi sistemlerin çok daha katmanlı olması gerekliliğini getiriyor. \
Temel olarak her validasyon eğer oynanışı etkiliyorsa server'dan geçecek. ClientNetworkTransform veya OwnerNetworkAnimator aslında client tarafından animasyon ve transformların set edilmesini sağlayacak ek sciptler,\
bunlar da daha hacklenmeye kapalı oyunlar yapmak istiyorsanız server tarafından denetlenmeli ve "Server Authoritative" tasarım seçilmeli. \

-NetworkBehaviour class and some of its functions (OnNetworkSpawned(), isServer isOwner bool etc)\
-NetworkObject, NetworkManager's functions: StartHost(), StartClient()\
-Spawning Network Objects \
-Network Friendly data sharing: NetworkObjectReference, basic value types, local functions that aid this process. (index > scriptableObject, scriptableObject > index etc)\

***Translate this
2) networkBehavior olan objelerin yok edilmesi, instantiate edilmesi, parent ataması yapılması ve hiyerarşideki yerlerinin değiştirilmesi gibi operasyonlar 
server tarafından diğer clientlara bildirilmesi gerektiği için buna yönelik bir tasarım yapıldı. Bunları içeren tüm logiclerin validasyonu ve/veya bildirimi yavaş yavaş ServerRPC'lere çekiliyor. 
ServerRPC'ye arguman göndermenin de bazı kısıtlamaları var, normalde olduğu gibi her tipten veriye ve her scripte erişmesi kolay değil. Dolayısıyla hem int float gibi basit veri tipleriyle çalışması sağlanmalı
Hem de NetworkObjectReferance'lar aracılığıyla diğer classlara erişilmeli. Bu da normalde kısacık kodlarla çözülebilecek sorunları 2-3 misli satır ve 2-3 farklı metoddan geçerek çözülmesini sağlıyor. 
Son derece basit olsa da bir liste içinde lokalde gerekli veriyi tutmak ve index aracılığıyla bu veriyle ulaşmak makul bir çözüm oluyor. 

```csharp
   [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }
```

-Multiplayer Game Logic flow: How to use timers in online and when to trigger serverRPC/ClientRPC.\
***Translate this
3) Temelde diğer oynayıcıların da görmesini istediğimiz ve client tarafından tetiklenen olayların koddaki düzeni:\
-1)localPlayer/Client bir logici tetikler ve ServerRPC'ye yönlendirir\
-2)ServerRPC valide eder ve/veya NetworkObject davranışlarıyla ilgili bir duruma müdahale eder ve/veya clientRPC'ye yönlendirir\
-3)ClientRPC görsel olan tepkiyi verir, örneğin lokal kodda bir eventi tetikler ve/veya bir animasyon oynar, UI açar \
FollowTransform.cs'in gerekmesinin sebebi, ServerRPC'den geçen ve referans olarak verilen Parent Object'in classına erişsek bile Transform bilgisinin eksik kalması.\
Bu yüzden bu gibi logiclerin baştan düşünülmesi gerekebiliyor. \

-NetworkVariable<T> for value types, including enums. Great for state machine design pattern and holding float timers. Comes with its own event .OnValueChanged \
-Usage of Dictionary<TKey, TValue>, for player status registiration\
-Logic that includes clientID's: serverRPCparams, NetworkManager.ClientIDs\

```csharp
 private Dictionary<ulong, bool> playerPausedDictionary;

 private void TestGamePausedState()
    {
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if(playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId])
            {
                //this player is paused
                isGamePaused.Value = true;
                return;
            }
        }
        //all players are unpaused 
        isGamePaused.Value = false;
    }
}
```
****Translate this
-Monobehaviour'daki execution order'a ek olarak OnNetworkSpawned gibi Netcode'a özel execution fonksiyonları da göz önünde bulundurulmalı, event takibi gibi işlerin nerede yapılacağında dikkat edilmeli.\


-Handling Disconnects via NetworkManager's "OnClientDisconnectCallback":

-Handling connections via NetworkManager's "OnClientConnectedCallback":

****Translate this
- Bağlantı kopukluklarında oyunun çökmesine veya tepkisiz kalmasına engel olmak için NetworkManager.OnClientDisconnectCallback'e sub olup kopukluk durumunda tetiklenecek bazı logicler yazıyoruz. 
Bu logiclere fonksiyonel(pause/unpause/ana menüye dönme özelliği), görsel-boyutsal (gameobject yoketme), UI'sal(bağlantınız koptu uyarısı verme) önlemler sayılabilir.


-More functions of the Network Manager: Connection Approval 
```csharp
	public void StartHost()
		{
			NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
			NetworkManager.Singleton.StartHost();
		}
	//...	
  private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }
        connectionApprovalResponse.Approved = true;
    }
```
Translate:
exp Network Manager özelliklerini daha derinlemesine keşfediyoruz. ConnectionApproval gerekliliği bağlanacak clientlara bir filtreleme opsiyonu sunuyor. 
ConnectionApprovalCallback, bir client bağlanmaya çalışırken request ve response struct'ı gönderiyor. Lokal logiclerle, scene seçimi, bağlı olan clientId'lerin sayısı gibi Response'a Approved değeri atayabiliyoruz. 
Bu oyun için yapmadık ama ConnectionApprovalRequest struct'ından gelen networkID'yi banlanmış oyuncular listesinde aratarak eğer içerdeyse almayabiliriz. Aklıma gelen bir kullanım oldu. 

-Scene flow handling in multiplayer projects: NetworkManager.SceneManager.LoadScene(), SceneManager.OnLoadEventCompleted, NetworkObject.SpawnPlayerObject()

Translate:
exp: NetworkObject'lerin daha detaylı kullanılması, NetworkObject class'ı detaylı incelenebilir. Scene flow eklemesi yapınca lifetimelar dikkate alınmalı. 
Gerekli şeyler için dontdestroy on load veya event takip eden ama tek scenede kalan şeyler için OnDestroy'da eventten unsub olmalı. (beklenmedik davranışlar olmasın diye ve memory leake engel olmak için)
Benzer sebeplerle oyundan ana menüye dönünce instance'lar, singletonlar silinlemi, static eventler resetlenmeli, networkManager shutdown edilmeli.
Ayrıca bağlantı birkaç saniye sürebildiği için join tuşuna bastığınızda kullanıcıyı uyarıcak UI elemanları göstermek önemli, bağlantı hatası gibi durumlar da bildirilmeli. bu yüzden event sistemleriyle UI emareleri kullanılmalı. 


-Using Struct's to share data in multiplayer game logic. 
Translate: 
exp: Singleplayerdakinin aksine verilen oyuncular arasında paylaşımının kolay olmaması yüzünden Struct'larda tutulması ve Network Listesinde yer alması son derece kolaylık sunuyor. 
Ayrıca bu listedeki değişimleri takip etmek için  NetworkList<T>.OnListChanged eventi oldukça kullanım kolaylığı sağlıyor. Herhangi bir veri değişiminden listeyi takipte olan bütün elemanlar kolayca haberdar oluyor. 
Ekrandaki dummylerin gösterilmesi, isimlerinin anı anına yazdırılması, sonraki güncellemelerde eklenecek renk değişimi gibi her şey bu sayede kolayca uygulanıyor.
-Structs can only hold value types, you need auxiliary functions to help them reach their full potential: 


```csharp
public struct PlayerData : INetworkSerializable, IEquatable<PlayerData> 
{

    public ulong clientId;
    public int colorId;
    public FixedString128Bytes playerName;
    public FixedString128Bytes playerId;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerId);
    }

    bool IEquatable<PlayerData>.Equals(PlayerData other)
    {
        return 
            clientId == other.clientId &&
            colorId == other.colorId && 
            playerName == other.playerName && 
            playerId == other.playerId;
    }
}
//...
 private NetworkList<PlayerData> playerDataNetworkList;
//...


```

*** REWRIITING STRUCTLIST SNIPPET (for color ID)

Translate:
exp: Struct veri tutmak için iyi olsa da gene serialization gerekliliği ile geldiği için kafamıza göre her veriyi tutamıyor. Bu yüzden gerekli olan dataya erişmek için yan fonksiyonlar üretiyoruz.
Örneğin düz color verisi tutamadığı için renkleri bi listede tutup bu listede refere edilen renklerin indexlerinden erişiyoruz. Commit'in çok büyük bir kısmı bu erişim sorunlarını halletmek için başka bir veri tipi döndüren fonksiyonlara atanmış.

***Example of aux funcions snippet


-Can custumize StartHost() and StartClient() so that host and client can respond differently to a callback or event. 

Translate: 
Commitle ilgili yorumum: IsServer durumunda bir kickPlayer özelliğinin gelmesi host ve clientlar için custom UI boxlarının gösterilebileceği fikri oyunu zenginleştirmek için kullanılabilir. 
aynı eventin server ve clientta farklı sonuçları olması da iyi bir kullanım. 

UNITY LOBBY ADDED
-Using Unity's official Lobby package, understanding basic concepts about lobby
-LobbyService class methods and properties: SendHeartbeatPingAsync(), CreateLobbyAsync(), UpdateLobbyAsync(lobby.id, updateLobbyOptions), QuickJoinLobbyAsync(), DeleteLobbyAsync() etc.  
-Lobby class and its properties: .HostId, .Id, .Data[TKey].Value, 
-AuthenticationService class methods and properties: SignInAnonymouslyAsync(), IsSignedIn, PlayerId 
-Searching Lobbies: LobbyService.QueryLobbiesAsync(), QueryFilter class, queryResponse class
-Async methods with await

***LOBİLER İÇİN ÖRNEK KOD SNIPPET KONABİLİR

exp: Lobi kullanımı için gereken prosedürlerin uygulandığı temiz bir örnek olmuş. Lobilerin neyden sorumlu olduğu, ne gibi özelliklerinin olduğu ve ne gibi etmenlerden etkilenebileceğinin üstünkörü uygulaması oldu.
Başta çok karışık ve meşakatli gelmişti. Adım adım kodun üstünden geçince anlaşılmıcak bir kısmının olmadığını ama prosedürlerin gerekliliği ve Lobby kütüphanesinden gelen çok fazla fonksiyonu direkt kullandığımız için hakimiyet kazanmadığımı anladım.
Bence son derece kolay bir şekilde kullanılabiliyor. Ezberlemek veya daha derin kullanımlara şimdilik uğraşmamın anlamı yok. 
Normal oyun kodlarkenki karşılaşılanın aksine çok fazla abstractlaştırılmış fonksiyonların olması bir oyun geliştirici olarak değil belki ama yazılımcı olarak OOP'yi daha iyi anlamamı sağlıyor. 

 
UNITY RELAY ADDED
-Using Unity's official Relay package, understanding basic concepts about relay\
-RelayService class methods and properties: CreateAllocationAsync(), GetJoinCodeAsync(), JoinAllocationAsync()\
-Using Allocation for setting server data of the UnityTransport \
-Task<T> return type for async funcions \

Example code that uses lobby and relay services together.
```csharp
     public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
        try {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiplayer.MAX_PLAYER_AMOUNT, new CreateLobbyOptions {
                IsPrivate = isPrivate,
            });

            Allocation allocation = await AllocateRelay();
            
            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                    {KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            }) ; 

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            KitchenGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
    }
```
exp:Özellikle Task<T> döndüren async fonksiyonlar bence programlamamı bir adım öteye taşıyabilecek yapılar. OOP ve yeni kütüphanelerde çalışmanın başka bir örneği daha. 
Bütün bu dersle beraber online oyunların yapısıyla alakalı daha çok şey öğrenmiş oldum. 

-Extra validations, null checks or double checks needed to prevent bugs in multiplayer where connection isnt always perfect

/WORK IN PROGRESS/