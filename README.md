# CodeMonkey2023-Overcooked-Clone Online with netcode / Lobby / relay

/WORK IN PROGRESS/
 This is the multiplayer complete tutorial covered by Code Monkey in this video: https://www.youtube.com/watch?v=7glCsF9fv3s
This game project started as a Singleplayer only game, then netcode support was added later on, I've also uploaded the complete sinpleplayer repo in github: https://github.com/OzgenKoklu/KitchenChaosSingleplayer
In this readme, I only mention the Netcode/Lobby/Relay related features and coding, the repository for Singleplayer project include the details on main game mechanics.

Disclaimer: All assets were borrowed from Code Monkey and are not used for commercial use, this is a learning project and I've built this on my own under instructions by Code Monkey. 
This built wont run on your computer as is unless linked to Unity Services, to link go to Edit > Project Settings > Services and link to a project in which Lobby and Relay services are enabled.

ABOUT THE GAME: This is a small scale casual game that is highly inspired by OverCooked, where you have to deliver specific orders in a time limit. 
It is a complete package with scene cycle, sound and animation, options menu, savable settings and key bindings with added multiplayer! 

/IMAGES AND VIDEO TO BE ADDED/
Screenshots: 
-main menu ss
-lobby list ss
-lobby scene ss
-Game play ss

Videos & Gifs: 
-Lobby color change gif
-Gameplay gif 

-YT Link: 

TLDR: What I've learned from this project:
-Implementing Netcode For Gameobject , Lobby & Relay by Unity to an already existing singleplayer game
-Basic understanding of multiplayer game logic: Server/Client authoritative design meaning, RPC's, basic solutions for bad connection
-Making a fully working game scene flow using netcode for gameObjects, then using lobby and relay services to connect online
-

On my decision to seperate the project in two repositories:
Short answer: The source code changed too much. And since I'm much new to multiplayer development, I'm not much familiar with structures and design choiches that were implemented in this current state.
Before this project, I havent done any multiplayer game project, I've heard of Photon Network and I think it still is a very popular option to make a multiplayer game.
The architectural needs of a multiplayer game is completely different and the programmer has to decide whether the game would be server authoritative or client authoritative and this alone changes the structure of the project. 
In singleplayer games, especially when you are in the very beggining in your developer journey, you only deal with whats on the screen, you make things change in behaviour to make the game play, however, 
the entire communication has to be tought out clearly for multiplayer development. Theres some essence of backend development in this sense where requests should be dealt with in particular ways. 
This project helped me further understand the C# and OOP concepts, I've understood while building it, but yet I feel unfamiliar still.

More about this project for those who have more time to spend: 
The project is a course project by Code Monkey and has its curriculum in this link: https://unitycodemonkey.com/kitchenchaosmultiplayercourse.php

However, I want to re-visit my own commits and make my own list of what I've learned: 
1)NetworkBehaviour, inherits Monobehavior, needed for networkObjects to work. Holds Network methods, properties and fields like IsHost etc.
All things gameplay related needs to be synced to other players somehow. This is one of the ways.
2)Server authoritative / Client authoritative desing. Programmer needs to decide whether code is validated on server side or client side. For casual game like this its OK to go with client authoritative, but its prone to hacks, not ideal for hardcore games.
3)ServerRPC / ClientRPC and the way they are deployed. ServerRPC only runs on server while ClientRPC runs on clients too. 
Validations, instantiation of network objects(SpawnKitchenObjectServerRpc), and syncing of method execution is handheld in serverRPC while the final execution of the logic is generally happens in Client side. 
4)IsServer bool is super useful to make some logic run just on server to avoid sync issues, IsOwner is great for doing some local logic like animation
5)In RPC's you can ony use Serializable parameter, thus sending int value index's work, but sending custom scriptable objects dont*
Making int/index returning functions, or reverse of that, returning class from int.
6)Singleton's cant be NetworkBehaviour, because there will be multiple instances of them in runtime. 
Player wont be a singleton but it can use static events for similar results, or have a LocalInstance. 
7)NetworkManager's StartHost(), StartClient(), etc 
8)Spawning Kitchen Object On Network Logic Expanation Step by step: 
..
Destroying KitchenObject via RPC's: 
..
9)Animation Syncing via RPC's Explained step by step:
..
10) Network Variables and details about their usage. 
..
11) Syncing game states, scene flow in multiplayer (lobby readdy, start game etc, pause state) 
12) PlayerData struct is shared through KitchenGameMultiplayer, usage of INetwrokSerializable and IEquatable 
13) Lobbies for connecting players. Awaitable Async methods, lobby related terminology such as queryResponse, queryFilter, heartbeat signals
...
14) Relay / Allocation to the host, Task<T> type as an async return type. 
...

ABSTRACT NOTES//
Netcode kısmı scriptlerdeki notlar, genelde değiştirilmiş scriptlerdeki metodların anlamlarını kendime not çıkarıyorum
****COMMIT 1 - a0367b3
- ClientNetwork tranform: client authoritative desing için dışardan kurduğumuz paket. sadece bir bool'u override ediyor
- Delivery manager artık networkBehaviour, spawnlama kısmı update'de ve sadece isServer ise çalışıyor
Spawn için bir int seçiyo rasgele ve clientRPC'ye gönderiyor. Client tarafında da bu intteki waitingRecipeSO waitingRecipeSOIndex'e ekleniyor ve 
OnRecipeSpawned invoke ediliyor. DeliverRecipe metodu eğer sonuç doğruysa DeliverCorrectRecipeServerRPC'yi çalıştırıyor. 
Eğer yanlışsa da incorrect recipeserverRPC çalışıyo. onlar da kendi içlerinde clientRPC'yi haberdar ediyorlar.
-KitchenGameManager.cs'de oyunun stateini hep countdownToStart olması için ayarlamışız. (test için)
-OwnerNetworkAnimator.cs de client autoritative tasarım için eklediğimiz script.
-Player scriptinde player network behaviour olmuş. Singleton instance'ı silmişiz ve localInstance tanımlamışız. Static eventler yazmışız (ONANYPLAYERSPAWNED ve ONANYPICKEDSOMETHIN)
Static datayı resetleme metodu eklemişiz. OnNetworkSpawn()(networkBehaviour'dan gelen bağlanma sonrası excecute edilen metod)'da IsOwner olup olmamasına göre local instance'ı ayarlıyoruz.
Ve onanyplayerSpawned'ı invoke ediyoruz. Update'de IsOwner ise movement ve handleInterictionsı yapıyoruz. 
Onanypicked'i de kitchenObject elimize alırsak yapıyoruz (setkitchenObject içinde). ayrıca gameInput'u tut çek bırak yapamıyoruz artık (çünkü network player object olarak spawnlanıyor) 
o yüzden static instance'ını kullanıyoruz.
-PlayerAnimator artık isOwner ise çalışıyor sadece. (başka playerların animasyonlarını etkilememek için.
-ResetStaticDataManager artık playerın static datasını da resetliyor. (eventler ekledik diye)
-SelectedCounterVisual artık lokal instance'ın eventini takip ediyor, bunun için start'ta player.localInstance'a bakıyo, eğer boş ise yeni bir spawn olunca tekrar bakıyor
Lokal instance boş değil ise lokal instance'ın OnSelectedCounterChanged'ine sub oluyor. Zaten singleplayer logicine göre çalışıyor geri kalan kod da, diğer clientlar bizim selected counterımızı görmüyor
-SoundManager artık player'ın OnAnyPickedSomething'ini takip ediyor, sender'ın player.cs'ini alıp o player'ın transformundan ses çıkartıyor. 
-testingNetcodeUI'da test için host ve client buttonları yapıp lambda expression ile network manager'ın host'unu veya clientını başlatma metodlarını aktive ediyoruz.

Commitle ilgili yorumum: 

****COMMIT 2 - 2116c71
-prefablerde networkObject eklemesi
-IKichenObjectParent'a GetNetworkObject() eklendi. böylece kitchenObjectParentlar null da olsa bir network object döndürebilecek.
-BaseCounter'a ve dolayısıyla tüm varyantlarına GetNetworkObject() metodu eklendi. 
-KitchenGameMultiplayer.cs eklendi, networkBehaviour. Singleton instance'a sahip, kitchenObjectListSO'ya sahip, 
NetworkObjeler client tarafında instantiate edilemediği için Spawnlama veya yok etme işlemlerini yapacak bir serverRPC gerekiyor.
Burada spawnlanacak objeyi ve parent'ın bilgilerini alacak bir fonksiyon yazıldı, bu lokal func KitchenObjectSO ve IKitchenObjectParent girdisi alıp
sonra bu girdiden lokal olarak index ve IKitchenObjecParent.getNetworkObject ile ServerRPC'ye uygun parametrelerle serverRPC'yi çağırıyor.
serverRPC'ye parametreleri göndermek için IKitchenObjectParent'a GetNetworkObject() eklemiştik. 
Aynı zamanda KitchenObjectList'e indexle ulaşmak veya KitchenObject değerinden index almak için iki fonksiyon yazıldı(bu scriptin altına), çünkü serverRPC'ler sadece data type ile çalışabiliyor 
/Dipnot: veri paylaşımı için Network serializable olmalı.
 SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
1) önce index değerinden bir KitchenObjectSO ya erişiyor ve bu SO'nun prefabini instantiate ediyor. (index değerini çekebiliyoruz çünkü args olarak)
2) instantiate edilmiş prefabin KitchenObjectnetworkObject Componenti ile bir networkObject oluşturuyor (networkObjenin spawnını kullanmak için)
3) networkObject.Spawn(true) ile bu objeyi online olarak spawnlıyor. (clientlarda da görünüyor böylece)
4) instantiate edilmiş prefab'in KitchenObject komponentini tutuyor (set kitchen object parent yapabilmek için)
5) Parametre olarak gelmiş parent olacak objenin NetworkReferansından TryGet ile NetworkObjesini tutuyor (interface'e erişmenin yolu olarak)
6) NetworkObjesinden de getcomponent ile IKitchenObjectParent interface'ine ulaşıp onu tutuyor (hala serverRPC'deyiz). 
7) KitchenObject'in SetKitchenObjectParent'ına bu interface'i parametre olarak gönderiyoruz(Server RPC içindeyken)
Not: Server RPC'nin içindeki kodun büyük bir kısmı kolayca refere edebileceğimiz komponentleri başka şekillerde erişmeye çalışarak büyüdü. 
GetKitchenObjectSOIndex SO'dan bir int döndürür. Listenin indexOf metodunu kullanır, GetKitchenObjectSOFromİndex ise o indexteki KitchenObject'i döndürür 
-KitchenObject.cs artık kitchenObject de bir networkObject, spawnKitchenObject sadece KitchenObjectMultiplayerdaki metoda SO ve parent bilgilerini gönderiyor 
transformu eşlemeyi bu committe silmiş. bundan sonraki committe düzeltiyorduk. Şu an spawnlanan objeler yerde spawlanıyo çünkü kitchenObject.cs'te iki satır comment out edildi
-Player scripti de artık network object döndürüyor. (çünkü IKitchenObjectParent olduğu için)
-KitchenObjectListSO'da tüm kitchenObjectlerin listesi var bu sayede indexleriyle ulaşabiliyoruz.

Commitle ilgili yorumum: 

****COMMIT 3 - b680efc
-Counterlara da network object eklendi böylece null döndürmücekler. 
-BaseCounter artık NetworkBehaviour dolayısıyla gameObjectinde de networkObject componenti var ve onu döndürebiliyor null yerine.
-ContainerCounter animasyonunun diğer oyuncularla senkron çalışması için serverRPC ile yapılmak zorunda. Player elinde kitchenObject yokken etkileşince kitchenObject spawn olur 
ve InteractLogicServerRPC() çalışır. içinde sadece server tarafına iletilen ve diğer clientlara da iletilmesi istenen animasyon oynatma mesajı var(OnPlayerGrabbedObject)
KitchenObject.SpawnKitchenObject zaten serverRPC'ye yönlendiği için client tarafından yapılmasında sakınca yok ve değiştirilmedi. 
-PlatesCounter.cs'de bir timer aracılığı ile plateler spawn ediliyordu. Update sadece serverda çalışack (IsServer). Plate spawn etme gene bir spawn işlemi olduğu için server RPC'ye yöneliyor
Timer server tarafında sayar, zamanı geldiğinde spawnPlateServerRPC çalışır o da client'da bu işlemin gerçekleşmesini söyler. Client tabak miktarını arttıp Onplatespawned eventini invoke eder
Burda da plate alma işlemi Container'daki gibi gene serverRPC ile olmalıdır. ServerRPC burda Client'a mesaj gönderir ve client tarafı da bir adet tabak azaltıp onPlateRemoved eventini invoke eder.
-FollowTransform.cs artık KitchenObject.cs'te SetParent aşamasında hallettiğimiz transform takip etme işini Netcode'da NetworkObject'lerin yapısından dolayı değiştirmek zorunda kaldığımızdan yazdığımız bir class
targetTransform tutuyor, bu KitchenObject'lere ekli, SetTargetTransform ile bir transform alıyor ve late update'de pozisyonunu eğer null değil ise bu target posisyona eşliyor.
-KitchenObject.cs içinde IKitchenObjectParent tutuluyor, ve followTransform.cs tutuluyor. Awake'de followTransform'u getComponent'la alıyoruz. 
SetKitchenObjectParent'ı direkt serverRPC ile yapıyor artık ve tek yaptığı şey bir IKitchenObject argümanı alıp ServerRPC'ye bunun NetworkObject'ini paslamak
ServerRPC ise bu networkObje referansı ile ClientRPC'ye paslıyor. ClientRPC'de NetworkObjectReferansından networkObje türetiliyor
NetworkObjeden interface'e erişiliyor, kendi kitchenObjectParent'ına girip onun kitchenObjectini siliyor ve kendi kitchenObject parentını değiştiriyor
en son followTransform.SetTargetTransform ile yeni kitchenObjectin transformuna eşliyoruz. 
-PlateKitchenObjede de awake'de base.awake'i aldık. Çünkü kitchenObject artık folowTransform'un komponentine orda erişiyor.

Commitle ilgili yorumum: 

****COMMIT 4 - 8939ac1
-KitchenObject artık DestroySelf'de ClearKitchenObjectParent() yapıp objeyi yok ediyor (client tarafında ama network obje olup serverda bu kod çalıştığı için diğer oyuncularda da kaybolacak)
-KitchenGameMultiplayer'da DestroyKitchenObject metodu var ve bu ServerRPC'ye yönlendiriyor, bir network Obje referansından kitchenObjeyi buluyor.
ClientRPCye ye bu referansı yönlendiriyor bu clientRPC'de parentını clear edecek). sonra da kitchenObject.DestroySelf'i çağırıyor(server tarafında).
ClearKitchenObjectParentClientRPC ise parametre olan networkObjeden kitcjenObjecti kenara yazıp onlara parentı silmesi için metodu çağırıyor KitchenObject.ClearKitchenObjectParent()
-Delivery Counter artık KitchenObject'in destroySelf fonksiyonu yerine yeni yazdığımız DestroyKitchenObject statik fonksiyonunu kullanıyor (serverRPC'li)
-Trash counter da artık destroySelf yerine online olanı kullanıyor ve Interaction sesi için serverRPC/ClientRPC kullanıyor (her oyuncu sesi duysun diye)

Commitle ilgili yorumum: 

****COMMIT 5 - 2b176a7
-Cutting Counter içersinde: Normalde eğer doğru bir tarif konulduysa interact içinde çalışan kodu tamamen serverRCP/clientRPClerle çalıştırıyoruz. 
Playerın kitchenobjectine yeni parent atıyoruz (bu counter) sonra bi object konduğundaki logic InteractLogicPlaceObjectOnCounterServerRpc(); de çalışıyor
Bu serverRPC de clientRPC'ye yönlendiriyor. Yaptığı şey cutting progressi sıfıra eşitlemek ve OnProgressChanged'i client tarafında invoke etmek (eventargs progressnormalized = 0) 
InteractionAlternate 'se artik cutobjectserverRPC ve TestCuttingDoneServerRPC'yi tetikliyor. CutObjectServerRPC >Client RPC'ye gönderiyor ve 
CutObjectClientRPC'nin içinde lokal olarak cutting progress artıyor ve anycut ve onAnycut eventleri tetikleniyo. sonra cuttingRecipeSO üstündeki kitchenObjectten alınıyor
UI için onProgress change tetikleniyor(burada cuttingProgressMax) için cuttingrecipeSO lazım. 
TestCuttingProgressDoneServerRPC ise eğer ki cuttingProgressMax'a eşitlenirse outputKitchenObject çıkartma logicini tetikliyor ve var olanı yok edip yenisini instantiate ediyor(server tarafında). 

Commitle ilgili yorumum: 

****COMMIT 6 - 43ec82d
-KitchenGameMultiplayer'daki SO'dan index, indexten SO döndüren fonksiyonlar StoveCounterda kullanılmak üzere privatedan publice çevirildi.
-StoveCounter.cs'de enum olan state bir networkvariable'da tutuluyor. NetworkVariable<State>, generic, her tür data type tutabiliyor. 
aynı zamanda frying timer ve burningtimer da artık NetworkVariable içinde tutuluyorlar. OnNetworkSpawn'da NetworkVariable<T>.OnValueChanged'lere sub olan fonksiyonlar yazıyoruz
aynı zamanda state.OnvalueChanged'i de takip ediyoruz. FryingTimer değişiminde>
Eğer fryingRecipeSO null değilse fryingTimerMax = fryingRecipeSO.fryingtimerMax, null ise 1f, sonra da IHasProgress için OnProgressChanged eventi tetiklenir.
progressNormalized = fryingTimer.Value/fryingTimerMax 
BurningTimer değişiminde de aynı algoritma kullanılır. State değişiminde ise lokal OnstateChanged tetiklenir. ve state'in burned veya idle olması durumunda bar yok olsun diye progress 0'lanır.
Update sadece IsServer ise çalışır. Eğer stove'ûn üstünde bir KitchenObject varsa stateMachine üstünden logic  döndürür.
Burada singleplayer kodundan farklı olarak NetworkVariable'ları enumlar dahil xxx.Value şeklinde yazmamız ve yeni destroy / spawn kodlarını kullanmamız sayılabilir.
bir de fryingTimer dolduğu zaman SetBurningRecipeSOClientRPC'i çağrılır clientlara SOIndex'i gönderilir) 
BurningTimer veya fryingtimer networkvariable oldukları için zaten değişimlerini takip edip IHasProgress için client tarafında da güncelleniyorlar. 
Interact fonksiyonu içinde artık KitchenObjectParent'ı set etmenin yeni fonksiyonunu kullanıyoruz ve interact Logicini gene serverRPC ile başlatıyoruz.
Interact obje yerleştirince server fryingTimer'ı 0lıyor statei değiştiriyor ve client tarafına fryingRecipe'yi set etmelerini söylüyor. 
State'i değiştirmek de serverRPC ile yapılabiliyor tam nedenini hatırlamıyorum acaba NetworkVariable'ların tamamı mı öyle idi acaba? 
BurningRecipeSO ve FryingRecipeSO'yu client tarafına iletmemizin sebebi client tarafında ProgressBarUI'ı tetiklicek eventin argümano olarak fryingtimerMax/burningtimerMax olması. 

Commitle ilgili yorumum: 

****COMMIT 7 - ac09cdd
-ClearCounter.cs ve CuttingCounterCounter'da yenilenmiş destroy fonksiyonu kullanımı eklendi.
-StoveCounterda da yenilenmiş destroy eklendi ve State'i idle'a eşleyen func kullanıldı bir yerde (bir fark yok çünkü networkVariable zaten
-PlateKitchenObject içersindeki tryAddingredient geçen committe çalışmaz hale geldi çünkü plate içersinde gösterdiği ingredientları bir listede tutuyor ama 
client tarafında interact çalışınca bu liste clientta güncellenirken diğer clientlar habersiz kalıyor. 
bu yüzden malzeme ekleme ve OnIngredientAdded eventlerini direkt client tarafında yapmak yerine bir serverRPC'ye eklenecek malzemenin indexi iletiliyor
sonrasında o da aynı indexi bir clientRPC ile diğer clientlara iletiyor. Bu clientRPC içersinde de listeye eklenip onIngredientAdded tetikleniyor.

Commitle ilgili yorumum: 

****COMMIT 8- e3fd9e9e40fd9e792c90d11ff319b1cd2c14f415
-Player prefab'ine mask layer(player)/yeni box collider eklendi çünkü collision için kullanılacak (collisions layer mask hem counters hem players, counters layer mask sadece counters)
-KitchenGameManger da artık networkBehaviour ve oyun state'ini bir NetworkVariable olarak tutuyor
Local player'ın hazır olup olmadığını tutuyor. CountdownToStartTimer, gamePlayingTimer da artık networkVariable, 
Ayrıca bir Dictionary(playerReadyDictionary) içinde ulong clientId'lerle birlikte IsReady bool'u tutuluyor. Ayrıca lokal playerın stateinin değişmesi de OnLocalPlayerChanged eventini tetiklicek
Awake'de dictionary'i initialize ediyoruz ediyoruz. OnNetworkSpawn'da state'i takipe alıyoruz ve State_OnvalueChanged'i tetikliyoruz o da 
lokal olan OnStateChanged'i tetikliyor. Input / interact butonu playerı ready yapıyor, OnLocalPlayerReadyChanged'i tetikliyor ve SetPlayerReadyServerRPC'yi tetikliyor.
SetPlayerReadyServerRPC serverRPCparams = default ile geliyor (buranın içinden SenderClientID'yi alabiliyoruz). Bu ServerRPC tetikleyen oyuncunun clientID'sini alıp Dictionary'de bu ulong'u true'ya eşitliyor
Sonra bir foreach'e giriyoruz, Network.Singleton.ClientIDs'deki tüm clientID'leri döndürüyoruz. 
Eğer networkManager'dan gelen clientID bu localDictionaryde yoksa veya lokal dictionary herhangi bir clientID için false ise her client hazır değil demek, çıktık. 
Eğer her client hazır ise game state'i değiştirip CountdownToReady'e alıyoruz. 
Update artık sadece Server'da çalışıyor. zaten timerlar hep NetworkVariable. zaten state de networkvariable olduğu ve StateOnvalueChanged'i takip ettiğimiz için bikaç event triggerlamayı silmişiz.
Bir de localPlayer'ın hazır olup olmadığını döndüren bir bool metod yazmışız. (tutorial UI'ı ve başka oyunculara ready olduğumuzu göstermek için hide etmek için) 
-Player.cs'te diğer oyuncularla collision için layermask ve spawnPositionListesi koymuşuz. Spawn olduğunda transform.position'ı spawnPosition listesini[(int)OwnerClientId] si ile belirliyor.
bu sonra bağlantı sorunlarında sıkıntı çıkartıyordu ama önemli değil şimdilik. HandleMovement fonksiyonu içinde canMove bool'unu hesaplarken CapsuleCast yerine boxCast kullanmışız ve collisionsLayerMask parametresi alan versiyonu kullanmışız.
Ayşı şekilde eğer !canMove ise X ve Z eksenlerinde hareketi çek ettiğimiz kodlarda da aynı layermaskleri kullandığımız boxcastleri koymuşuz. 
-TutorialUI.cs artık tutorial'ı göstermek için oyun state'ine bakmaktansa OnLocalPlayerStateChanged'i takip ediyor ve buna göre kendisini saklıyor.
-WaitingForOtherPlayersUI.cs'de tutorilı geçip diğer oyuncuların hazır olmasını beklerken çıkan yazıyı çıkartan script. KitchenGameManager'ın hem OnLocalPlayerReadyChanged'ini hem de OnStateChanged'ini takip ediyor
State değişiminde eğer IsCoundowntoStartActive() ise kendisini saklıyor. Eğer IsLocalPlayerReady() ise kendini gösteriyor(tutorial UI'ı geçince yani)

Commitle ilgili yorumum: 


****COMMIT 9 - 8b31bcd
-KitchenGameManager.cs'te 4 yeni event. lokalPlayer'ın oyunu durdurup devam ettirdiği veya multiplayer oyunun durup devam ettiği üstüne
isGamePaused bool'u yerine isLocalGamePaused bool'unu yazdık. IsGamePaused diye bir NetworkVariable<bool> tanımladık. Oyunu başlatmak için kullandığımız dicitionary'nin benzerini bu sefer de 
playerPausedDictionary şeklinde tutuyoruz. (ulong / bool) ve awake'de onu da initialize ediyoruz. OnNetworkSpawned'da IsGamePaused.Onvalue change'i takibe alıyoruz.
Ve değiştiği zaman eğer IsgamePuased ise timescale'ı 0'lıyoruz OnMultiplayerGamePaused'u da invoke ediyoruz. Eğer !isGamePaused ise timescale = 1, OnmultiplayerUnpaused da invoke ediliyor.
ToggleGamePause() metodunun içinde isLocalGamePaused bool'u tersine çeviriliyor ve 0 veya 1 olmasına göre OnLocalGamePaused veya OnLocalGameUnpaused triggerlanıyor. 
Aynı zamanda PauseGameServerRPC() veye UnpauseGameServerRPC() de çalıştırılıyor. Servera lokalde durdurma olduğunu haber veriyoruz. 
ServerRpcParams aracılığıyla göndericinin clientID'si üstünden playerPausedDictionary'e o clientID'nin 1 veya 0 olduğu eşitlenir ve "TestGamePausedState" tetiklenir. 
TestGamePaused lokal bir fonksiyondur ama sadece Serverda çalışacak çünkü client'a bir emir gitmedi. bu da geçen commit'teki tüm clientların Ready olması algoritmasına benzer bir şekilde
ConnectedClientID'sdeki tüm client ID'lerin üstünden döndürülür, duruma göre herhangi birinin oyunu durdurduğunu veya kimsenin durdurmadığını döndürür. 
-GamePauseUI.cs'de sadece lokal oyunun pause olup olmamasını takip ediyoruz. event isimleri değişmiş.OptionsUI.cs'de de aynı şekilde. 
-PauseMultiplayerUI.cs ekranda "waiting for other players to unpause tarzı bir text gösteriyordu. OnlineMultiplayer'ın durdurulup devam etme eventlerini takip ediyor. 
Duruma göre show hide fonksiyonları devreye giriyor.

Commitle ilgili yorumum: 

****COMMIT 10 - 9f8bb1b
-KitchenGameManager.cs start'ta eğer isServer ise bağlantı kopukluklarını takip etmek için NetworkMakager.Singleton.OnClientDisconnectCallback eventi takibe alınır
Bu callback clientID döndürüyor. içersinde autoTestGamePausedState bool'u (yeni tanımladık) true'ya çekiliyor. bir frame atlanıp clientID'nin connectedID listesinden çıkmasını garanti altına almak için yapıyoruz.
LateUpdate'de de eğer isAutoTestPausedState true ise false'a çekiliyor ve TestGamePausedState Çalıştırılıyor (bu sadece server tarafında) 
Eğer bağlantısı kopan kişi oyunu pause edip koptuysa oyun bug'a girip paused kalırdı(bunu yapsaydık). 
-Player.cs'te OnNetworkSpawn'da eğer isServer ise gene OnClientDisconnectCallback'e sub olunur. Callback triggerlanınca da 
Eğer bağlantısı kopanın clientId'si bu playerınClientID'si ise ve bunun elinde bir KitchenObject varsa bu kitchenObjecti yok ediyoruz. 
Playerların hepsinde çalışan ama sadece server tarafında çalışan bir func. 
-GameOverUI.cs'te ise bu UI artık playAgainButton'a sahip ve lambda expression ile içine NetworkManager'ı kapatmasını ve main menu ekranına dönmesinin özelliği noymuşuz.
-GamePausedUI'ın içine gene ana ekrana dönerken NetworkManager'ı kapatmasının özelliğini eklemişiz (NetworkManager.Singleton.Shutdown();
-HostDisconnectUI.cs'de ise bir playAgain button'u var. lambda ile gameoverdakinin tamamen aynı şeyleri yaptırmışız.
Start'da bu NetworkManager.Singleton.OnclientDisconnectCallback'ine sub oluyor. 
Eğer event triggerlanırsa ve callbackten dönen clientId eğer serverClientId ise hostun bağlantısının koptuğunu oyuncuya söylüyor.

Commitle ilgili yorumum: 

****COMMIT 11 - 5c65bea
-NetwormManager componentinin connection approval boolu 1e alındı böylece herkesin bağlanmasına izin vermicek (oyun başlamışken vermicek)
-KitchenGameManager.cs'e oyunun isWaitingTo start olup olmadığını söyleyen bool func eklendi. (public)
-KitchenGameMultiplayer.cs'e bu zamana kadar TestingNetcodeUI.cs'de olan startHost startClient fonksiyonları eklendi. 
StartHost'un içinde ConnectionApprovalCallback'e sub oluyor ve network manager'dan hostu başlatıyor.
**ConnectionApprovalCallback > connectionApprovalRequest ve ConnectionApprovalResponse objeleri döndürüyor ve bu callback'ten tetiklenen eventte
Eğer oyun IsWaitingToStart ise connectionApprovalResponse.Approved ve .CreatePlayerObject 'i true'ya çekiyoruz. Eğer ki değilse false yapıyoruz.
StartClient'ta ise basitçe NetworkManager'ın StartClient fonksiyonunu kullanıyoruz. 
-TestingNetcodeUI.cs'de fonksiyonları direkt çağırmak yerine KitchenGameMultiplayer instance'ı üstünden çağırıyoruz. 

Commitle ilgili yorumum: 

****COMMIT 12 - f323821
-Scene flow eklendi. bunun için CharacterSelectScene ve LobbyScene eklendi. (main menu> Lobby> game), normalde bağlantı gameScene'de olduğu için bazı gameObjectlerin yerleri değişti vs.
-KitchenGameManager.cs normalde playerları networkManager'ın PlayerPrefab'inden spawnladığımız ama artık networkManager'ın host bağlandığında spawnlaması saçma olduğundan (scene flow geldi diye)
artık player prefab'ini kitchenGameManager spawnlıyor. İçinde prefab'e referans verdik. kodu çalıştıran server ise OnNetworkSpawn()'da DisconnectedCallback'e sub olurken
scene.Singleton.sceneManager.OnLoadEventCompleted'a da sub olur. bu load işlemi gerçekleşince bütün bağlı oan clientID'leri foreachle döndürür
Onlar için birer playerTransform'u instantiate eder ve NetworkObjectlerine erişip. SpawnPlayerObject(clientId,true) yapar. (bunu daha önce yapmak zorunda değildik çünkü NetworkManager hallediyordu)
-KitchenGameMultiplayer.cs'de max player amount const int olarak tanımlanmış. OnTryingToJoin ve OnFailedToJoin eventleri yazılmış.
Awake'de DontDestroyOnLoad aktif edilmiş, çünkü artık gameMultiplayer lobyde başlıyor, gameScene'e taşınıyor.
ConnectionApprovalCallback'ini dinleyen func Eğer Scene character select scene değil ise connectionApproval vermiyor., eğer bağlı olan Id'lerin sayısı max sayıdan fazlaysa da vermiyor. 
Diğer türlü approval veriyor. Client başlatırken ek olarak OnTryingToJoinGame invoke ediliyor ve NetworkManager'ın OnClientDisconnectedCallback'ine sub olunuyor.
Eğer bu callback'ten trigger gelirse de OnFailedToJoinGame eventi tetikleniyor.
-Loader.cs'in içindeki enum'a yeni scene adları eklendi (LobbyScene, Character SelectScene) ve artık LoadNetwork diye bir func üstünden NetworkManager içindeki
Scene Manager kullanılıp Load scene yapılıyor. Parametre olarak Scene targetScene alıyor. LoadSceneMode.Single var olan gameObjelerin tekrarlanmadan diğer sahneyi load etmesi için bir opsiyonmuş. bir de loadscene.Additive var
Additive loadscene sanırım mmo gibi bi şeyde faydalı olabilir. ek harita loadlamak için. 
Ek olarak loadingScene'i bypass ediyoruz network için yazdığımız oyunda. (1 frame siyah ekran gösteren bir scene'di)
-MainMenuCleanUp.cs awake'de networkManager singleton'ını siler, kitchenGameMultiplayer singleton'ını siler.
-Player.cs'te PlayerHeight'ını silmişiz zaten cast'ler içindi, kullanmamaya başladık.
-ConnectingUI.cs'te klasik show hide fonksiyonları var. Ekrana "Connecting..." yazdıran bir UI canvas. OnTryingtToJoingGame ve OnFailedToJoinGame'e sub oluyor. Duruma göre kendisini gösterip kapatıyor.
Lifetime'ı farklı olduğu için de OnDestroy'da eventlerden unsub oluyor. 
-ConnectionResponseMessageUI.cs de benzer bir class, sadece OnFailedToJoinGame'e sub oluyor. Ve eğer fail olursa
Show() yapıp ekrandaki mesajı NetworkManager.Singleton.DisconnectReason'a eşliyor. Eğer mesaj boşsa sıradan "Failed to connect" mesajı çıkıyor
-Main menu'de PlayButton artık Loader.Load game scene'dense lobbyscene'e götürüyor. 
-TestingCharacterSelectUI.cs'de oluşturduğumuz characterSelect scene'i i.inde ready buttonuna sahip bir awake. 
buttona basınca CharacterSelectReady'nin SetPlayerReady fonksiyonunu yapıyor. 
-TestingLobbyUI.cs'de CreateGame(host) ve JoinGame(client) butonları var. Bunlar KitchenGameMultiplayer.cs'deki hostu çağrıp Character select scene'e gidiyor
Veya client'ı başlatıyor. (henüz scene değiştirmiyor client. ??? Client serverda loadlu olan scene'i otomatik load ettiği için özellikle scenemanager kodu yazmamıza gerek yokmuş.
-CharacterSelectReady.cs bir networkbehaviour ve bir static instance'a sahip. awake'de playerReadyDictionary artık burda tutuluyor. 
Ready olup olmama algoritmasını kitchenGameManager'dan kopyalamışız. 

Commitle ilgili yorumum: 

****COMMIT 13 - 506cc44
-Character select scene için visual dummy bir karakter prefabi ürettik.
-Scene'e bu dummyleri koyup oyuncu sayısına göre active olacak şekle getirdik.
-PlayerData.cs bir struct. NetworkVariable'lara ek bir data paylaşma yolu. Network üstünden paylaşılması için bu struct'ın
INetworkSerializable ve IEquatable<PlayerData> olması gerekli. Bu interfaceler 2 adet fonksiyonla geliyor.
public void NetworkSerialize<t> struct içindeki tüm dataları Serialize ediyor. IEquatable ise bir bool döndürüyor. clientId ==other.clientId olup olmadığını. 
-KitchenGameMultiplayer.cs PlayerData struct'larından oluşan bir playerDataNetworkList tutuyor ve bu listeyi awake'de initialize ediyor. 
PlayerDataNetworkList.OnListChanged eventine de sub oluyoruz. Bu event triggerlanınca OnPlayerDataNetworkListChanged diye bir lokal eventi tetikliyoruz.
Ayrıca StartHost() içerisinde NetworkManager'ın OnClientConnectedCallback'ine de sub oluyoruz ve bu callback triggerlanınca
PlayerDataNetworkList'e callback'ten gelen clientId'yi ekliyoruz(yeni playerdata struct'ı üreterek). 
Ayrıca public bool IsPlayerIndexConnected diye bir func ile int üstünden sorgulama yapıp eğer indexin playerdataNetworkList.count'dan küçük kalıp kalmadığına bakıyoruz. 
Bu bool func'ını CharacterSelectPlayer'ın içinde kullanıyoruz(dummyleri göstermek için)
-CharacterSelectPlayer.cs'de bi playerIndex tutuyor. startta OnPlayerDataNetworkListChanged'e sub oluyor ve UpdatePlayer()'ı yapıyor, updatePlayer içinde o indexteki player'ın bağlı olup olmadığının verisi dönüyor.
-CharacterSelectUI.cs'de de main menu ve ready tuşları var. Main menu'ye basınca Loader'dan main menuscene'i açıyor.
Ready'e basınca da CharacterSelectReady'nin instance'ından setPlayerReady() yapıyor. 

Commitle ilgili yorumum: 

****COMMIT 14 - bd1b4a0
-Karakter rengini seçmek için UI elemanları ve scene değişimleri yapıldı. Character select scene'ine renk tuşları ve renkler eklendi
-CharacterSelectPlayer.cs'de PlayerVisual.cs ve readyGameObject referansları verildi. Startda artık OnReadyChanged takip ediliyor. 
OnReadyChanged'e göre görsel update ediliyor. Aynı zamanda UpdatePlayer artık Index üstünden PlayerDataya erişiyor(kitchengameMultiplayerdan)
CharacterSelectReady'den clientID üstünden ready olup olmadığını öğrenip ona göre readyGameObject'i show veya hide yapıyor. 
aynı zamanda playerVisual.cs'den de renk ayarlama özelliğini kullanıp bu playerData structında tutulan rengi seçiyor. 
-CharacterSelectReady.cs'de OnReadyChanged diye event yapmışız. (visual'ı update etmek için). Aynı zamanda SetPlayerReady sadece client'a haber veriyordu oyun başlasın diye
artık diğer oyuncular da görebilsin diye SetPLayerReadyClientRPC de kullanıyoruz. Bu da tüm clientların görsel olarak update almasını sağlayacak logic tetikliyor
Ready dictionary'i onlar için de güncelleyip OnReadyChanged'i tetikliyor. Ayrıca. IsPlayerReady(clientID) alan func yazmışız bu da characterSelectPlayer.cs'de kullandığımız fonksiyondu.
-KitchenGameMultiplayer.cs'de List<Color> tutuyoruz. ClientConnectedCallback'te bağlanan playera data oluştururken ilk kullanılmamış rengi veriyoruz struct'a (GetfirstUnusedColorID diye func yazmışız.
PlayerData'ya indexten erişim için func yapmışız. ColorId indexinden color döndüren func yazmışız(listeden). ClientId'den index döndüren func yazmışız
ClientId'den PlayerData döndüren func yazmışız. Bir de düz playerData döndüren func yazmışız. bu yazdığımız ClienId'den döndüren func'ı Network.Singleton.LocalClientID ile çağırıyor.
Color değiştirme (server rpc), Color'ın kullanılabilir olup olmadığına bakma, ve ilk kullanılmamış color'ı alma fonksiyonları yazmışız. 
ServerRPC önce parametre olarak gelen seçilmek istenen color'ın kullanılabilir olup olmadığına bakara değilse hiçbi şey yapmadan return yapar.
Eğer color müsaitse önce playerData indexini clientId üstünden (serverpcparams defaulttan gelen) çeker. bu indexte bir PlayerData yaratır. 
yarattığı struct'ın colorId'sini seçilen colorId yapar. ve sonra listenin çekilen indexinin üstüne bu structı tekrar yazdırır. Struct value type olduğu için bu aşamalar gereklidir. 
IsColorAwailable'da bi colorId parametresi alıyor. PlayerDataNetworkList'teki tüm PlayerDataları döndürüyor ve seçilen colorId'nin herhangi bi PlayerData.colorId'ye eşit olup olmadığına bakıyor. 
Foreach loop'undan çıkarsa true olarak dönüo. GeFirstUnusedColorId'de PlayerColorList.count'a kadar bir for döngüsü dönüyor. indisteki colorId'nin awailable olup olmadığına bakılır (yazdığımız IsColorAvailable func ile) 
Eğer müsaitse indisi döndürür.
-PlayerData'ya colorId eklendi ve onu da IEquatable ve NetworkSerialize'ın içine sokuyoruz. 
-PlayerVisual.cs ekrandaki dummylerin tekil olarak rengini değiştiren script. kafa ve vücut için iki farklı meshrenderer var bunları tutçekbırak ile almışız.
bir tane private Material material'ımız var awake'de bu material'ı meshrenderer'daki materyalden initiate ediyoruz. sonra head ve body'deki material'ı da bu materyale eşliyoruz.
SetPlayerColor'da da material rengini gelen color parametresine eşliyoruz. Bu fonksiyonu characterSelectPlayer.cs'te UpdatePlayer içinde kullanıyorduk o da liste değişimlerine filan sub olmuş durumda.
-CharacterSelectSingleUI.cs Ekrandaki color buttonlarına renk vermek için kullanıyoruz. Aynı zamanda button işlevi de taşıyor. ve kendine atanmış colorId ile ChangePlayerColor(colorID) tetikliyor
Startta OnPlayerDataNetworkListChanged'i takip ediyor. kendine atanmışcolorID ile kendi rengini alıyor KitchenGameMultiplayer'dan.
Sonra da seçilip seçilmediğini updateliyor. takip ettiği eventte de triggerlanınca gene seçilip seçilmediğini açıp kapıyor. 
Eğer lokal oyuncu bu colorID'yi seçmişse selectedGameObject'i açıp kapıyor. 

Commitle ilgili yorumum: 

****COMMIT 15 - 6c16e8
-ChangeSelectedPlayer.cs'de  Yeni bir kick buttonu koyduk start'ta isServer olmasına göre açıyoruz. Bunla tekil dummyler için olduğu için üstlerine atanmış indexler var
index'ine göre playerDataya erişiyoruz. PlayerData'dan da clientId'ye erişip KitchenGameMultiplayer'daki kickplayer özelliği ile atıyoruz. 
Aynı zamanda onDestroy'da bu obje takip ettiği eventlerden unsub oluyor. (lifetime meselesi)
-KitchenGameMultiplayer.cs'da OnClientDisconnectCallback'i server olarak ayrıca takip ediyoruz çünkü PlayerDataNetworkList'i modifiye edeceğiz. 
bu callback'ten bağlantısı düşen oyuncunun clientId parametresini playerData.clientID'ler arasında aratarak indisini buluyoruz ve o indisteki liste elemanını çıkarıyoruz.
Ayrıca bu scripte normal OnClientDisconnectCallback'i de client özelinde onfailedtojoinGame'i tetikleyen bir event haline getirmişiz. Server bağlantısı kopan client'ı playerdatadan silerken client ise sadece ekrana erör mesajı yazdırıyor.
KitckPlayer ise NetworkManager.Singleton.DisconnectClient(clientId) ile client'ın bağlantısını koparıyor ve normalde bağlantı kopunca datasını silme eventini kendisi tetikliyor. 
-Player.cs'de playerVisual scriptine referans var. artık gameplayde de playerin rengini ayarlıyoruz startta clientId'den playerdataya, playerdata'dan colorId'ye erişerek.
ayrıca SpawnPositionını artık playerData index'i alarak ayarlıyoruz. çünkü diğer türlü disconnectlerde sorun oluşuyordu.
-CharacterSelectSingleUI ve HostDisconnectUI artık ondestroy'da sub oldukları eventlerden unsub oluyorlar. (lifetimelar farklı diye)

Commitle ilgili yorumum: 

****COMMIT 16 - e2f19b2 UNITY LOBBY ADDED
-Unity'nin lobby paketi eklendi burda. sahnelere bir sürü UI objesi eklendi (text input filan için).
-Lobiler artık unity server üstünden bağlanmamızı sağlıyor. Şimdiye kadar olan sourcecode sadece localIP üstünden bağlanmamızı sağlıyordu.
-CharacterSelectPlayer.cs'de kickplayer özelliği aynı zamanda lobiden de karakteri atıyor(kitchenGameLobby.cs'den func ile). (clientId'ye göre)
Aynı zamanda UpdatePlayer'ın içine playerNameText de dahil edildi. PlayerData'da artık tutulan playerName stringe dönüştürüp karakterin üstündeki worldspaceUI'a yazdırılıyor
-CharacterSelectReady'nin içine tüm clientlar hazır olduğunda lobbyi silme komutu eklendi. (kitchenGameLobby.cs'deki func çalışıyor) Çünkü lobinin tek amacı server üstünden ilk bağlantıyı kurmak. Sonraki bağlantı için relay'i kullanacağız.
Lobby sadece playerların çeşitli özelliklerle oda kurmak ve odaları dünya çapında arattırmak filtreli/filtresiz aratmak, bağlanmak, buluşmak üstüne. 
****Lobilere dair çok önemli bilgiler burada, işin doğası gereği çok farklı classlar kullandık, çok kurcalama fırsatı bulamadan pek çok konsepti kullanıp geçtik.
****Bir back-end developer kadar öğrenmenin de çok anlamlı olduğunu düşünmüyorum o yüzden biraz kopyala yapıştır kod halinde kullanmakta pek sakıncası olmayan teknikler.
****Ayrıca çok iyi anladığımı iddia edemicem, anlamak için üstünden geçiyorum ama göreceğiz.
****Bir not daha, Authenticationservices ve lobbylerde bulunan bazı hazır fonksiyonların isimlendirmeleri filan baya kolay, kodu okuyunca ingilizce okumuş gibi anlaşılıyor zaten.
-KitchenGameLobby.cs Unity.services.Authentication'ı kullanan ve lobi işlerini halleden temel script. Bir singleton, Lobinin kurulmasına, kurulurken sorun çıkmasına
lobiye joinlenme girişimine ve girişimin faillanmasına dair eventler mevcut. Aynı zamanda mevcut lobilerin listesinin değişmesi durumunda tetiklenen bir event de var
Eventargs içinde de list<Lobby> gönderiyor. Katılınan lobiyi Lobby clası şeklinde tutuyoruz. (services.lobbies veya lobbies.models'dan geliyor)
Heartbeat konsepti lobinin backend'de ölmemesi için devamlı gönderilmesi gereken sinyal ile alakalı. Lobiyi kuran bu sinyali göndermek için timer tutuyor.
Yeni lobileri listelemek veya listeyi güncellemek için de timer kurmuşuz. Awake'de DontDestroyOnLoad diyoruz çünkü bu lobi objesi LobbyScene > CharacterSelectScene boyunca yaşamını sürdürücek.
Ve lokal yazdığımız InitializeUnityAuthenticaion() yapıyoruz. Bu bir Async fonksiyon yani tek cycle'da tamamlanmak zorunda değil. İşlem tamamlanınca koda olduğu yerden devam edecek.
Eğer UnityServices.State başlatılmamışsa: InitializationOption tanımlıyoruz. (bir class) içinden gelen .SetProfile(string x) ile Random bir isimle profil oluşturup giriyoruz.
Ve servisi bu random isimle başlatıyoruz. Aynı zamanda servera bağlanırken yaptığımız her şeyi await'li yapıyoruz çünkü tek framede gerçekleşmesi imkansız
ve server'dan bilgi gelene kadar işlemi askıya almamızı sağlıyor (async/await). AuthenticationService.Instance'dan da anonim olarak login fonksiyonunu çağırıyoruz.
Burada SignInAnonymouslyAsync()'ye ek Unity ID, appleId, playstore ID, steam ID veya konsollardaki ID gibi başka alternatifler de mevcut. 
Update'de heartbeat'i ve liste güncelleme fonksiyonlarını yapyıro.
HandlePeriodicListLobbies() eğer bir lobby'e katılmadıysak ve AuthenticationService'de IsSignedIn isek VE Scenemanager'daki sahne lobbyscene ise
timer sayar ve timer bittiğinde timerı tekrar ayarlar, ve lokal fonksiyon olan ListLobbies()'i uygular. 
ListLobbies() 
HandleHeartBeat() ise IsLobbyHost() durumunda heartbeat sayacını sayar, sayaç dolunca sayacı tekrar ayarlar ve
await LobbyService.Instance.SendHeartBeatPingAsync(joinedLobby.Id) hazır fonksiyonu ile bağlanılmış lobiye heartbeat gönderir. 
isLobbyHost() bool döndüren bir lokal fonksiyon, joinedLobby null değilse VE joinedLobby.HostID() AuthenticationService'deki PlayerID ile eşitse / 
YANİ KISACASI KODU ÇALIŞTIRAN KİŞİ HOST İSE 1 DÖNDÜRÜR. 
CreateLobby(String LobbyName, bool isPrivate) lobi yaaratmak için kullandığımız fonksiyon, 
OnCreateLobbyStarted eventini invoke ediyor, try catch içinde bağlanılmış lobiyi, isim, lobby kişi sayısı ve CreateLobbyOptions {Isprivate} ile oluşturduğumuz yeni lobiye eşliyoruz.
Bir yandan KitchenGameManager'dan paralel olarak Host'u başlatıyoruz. ve Character selectScene'e gidiyoruz. 
Catch durumunda herhangi bir lobbyServiceException'ı logluyor ve OnCreateLobbyFailed eventini invoke ediyor. 
Burada not olarak: Lobiler ve Netcode'daki Host/Client paralel olarak çalışıyor.
QuickJoin'de OnJoinStarted invoke ediliyor. Ve LobbyService'in QuickJoinLobbyAsync() fonksiyonu çalıştırılıyor, ve KitchenGameMultiplayerdaki StartClient() çalışıyor
Catchde de erörü yazdırıp OnQuickJoinFailed invoke ediyoruz. 
JoinWithCode(string lobbycode) JoinLobbybyCodeAsync(lobbycode) ile joinedLobby'i seçiyor ve client'ı başlatıyor. Catch aynı şekil öncekiyle. 
JoinWithLobbyId(string lobbyId) aynı şeyi JoinLobbyByIdAsync(LobbyId) ile yapıyor. 
DeleteLobby() ise JoinedLobby null deil ise LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id) ile lobbyi siliyor, local JoinedLobbyi nulluyor. (bunu host olarak çıkış yaparken kullanıyoruz)
LeaveLobby()'de LobbyService'in RemovePlayerAsync()'ini joinedLobby.Id ve AuthenticationService'deki PlayerId ile kullanıyoruz ve lokal lobbyi nulluyoruz.
KickPlayer(string playerId) Host ise LobbyService'in RemovePlayerAsync'ini lobyId ve playerId ile çalıştırıyo. 
Bir de GetLobby var diğer scriptlerde kullanmak için. 
KitchenGameMultiplayer.cs'de multiplayer ismimizi tutacak playerprefs key'ini const olarak yazmışız. PlayerName'ini privateString olarak tanımlamışız.
Awake'de PlayerPrefs.Getsring ile playername'ini set ediyoruz. Eğer yoksa da default value olarak bir randomRange generate ediyoruz. 
GetPlayterName ve setPlayerName fonksiyonları var. Set olanda ayrıca PlayerPrefs'de de kaydediyoruz. 
Client artık NetworkManager'ın OnClientConnectedCallback'ini takip ediyor. triggerlanınca da SetPlayerNameServerRPC ve SetPlayerIdServerRPC'yi tetikliyor. 
Birinde Server'a kaydedilmiş adıyla beyanda bulunuyoruz ve playerDataIndexteki playerData.playerName'i editleyip struct'ı tekrar o indise kaydediyoruz.
Diğerinde de aynı şeyi PlayerId için yapıyoruz. (authenticationService.Instance.PlayerId'den aldığımız). 
MainMenuCleanUp.cs'de KitchenLobby.Instance null değilse onu da destroy ediyoruz. 
PlayerData.cs'te struct'a 2 adet FixedString128Byte değişkeni daha tutturuyoruz. (playerName ve PlayerId(authenticationService'den aldığımız) String değil bunu tutturma sebebimiz string de serializable değil çünkü data type değil. 
FixedString hem sınırlı hem karakter isimleri için yeterli. 
CharacterSelectUI.cs'de Lobby ismi ve lobi kodu bölümü eklendi (host lobi kurunca görebiliyor). Main menuye dönerken artık KitchenGameLobby'deki LeaveLobby fonksiyonunu da uyguluyoruz.
Startta ise KitchenGameLobby'nin Lobby verisini alıyoruz. Ekrandaki lobi ismi ve kodu textlerini de lobby.Name ve lobby.LobbyCode'a eşliyoruz.
ConnectionResponseMessageUI'ın ismi değişti ve LobbyMessageUI.cs oldu. :
LobbyMessageUI.cs'de eskiden sadece bağlantı hatasında ekrana connection error yazdıran ConnectionResponseMessage'a ek KitchenGameLobby.cs'deki
OnCreateLobbyStarted, OnCreateLobbyFailed, OnJoinStarted, OnJoinFailed, OnQuickJoinFailed eventlerini de takip ediyor. 
Bağlamına göre mesajlar gösteriyoruz, OnDestroy'da eventleri lifetime meselesinden dolayı takipten çıkıyoruz. 
LobbyCreateUI.cs'de lobby scene'in lobi açmalı UI işlerini KitchenGameLobby.cs'deki fonksiyonlara bağlayacak özellikler var. 
Public ve Private lobby açmada parametreleri değiştirilmiş olarak inputfielddaki.text kısmıyla isimlendirilmiş lobielr açıyor.
LobbyListSingleUI vertical group yaptıpımız buttonların template'ine(tekil) lobi ismini yazdırıyor. ayrıca bunlar buttondu. SetLobby ile başka bir scriptten üstlerindeki text değiştiriliyor veya bu templateden oluşturulma objeler silinip oluşturuluyor. 
LobbyUI.cs'de genel olarak lobby scene'in UI işleri halledilir. ManinMenu, CreateLobbyButton,QuickJoin, joinWithCode buttonları
LobbyCreateUI'ın show hide fonksiyonları, lobbyCodeInput ve PlayerNameInput için ınptfield, lobi listelemek için de lobby container ve template referansları var.
Buttonlara ana menuye dönme, lobby'e basınca lobbyUI'ı açma, quickJoin ve join with code özellikleri atandı. 
Startta inputfield'ı KitchenGameMultiplayer'dan doldurup gösteriyor. Aynı zamanda inputField.onvalueChange'e de listener atıyor. Her değişimde KithcenGameMultiplayer.SetPlayerName(string s)'i kullanıyor.
Lobi listesi değişimine sub oluyor. ve lobi listesini yeni bir liste oluşturup güncelliyor. 
Lobi listesi değiştiği anda UpdateLobbyList(e.lobbyList) ile uygulanıyor. Burdaki UpdatelobbyList'in amacı ekrandaki verticalGroup'a yeni childObjeler spawnlayıp göstermek. 
ChildObje template ise geçiyor, değilse yokediyor(tekrar yüklemek ve doğru gösterdiğini garantilemek için). 
Foreach döngüsü içinde Lobbylistteki tüm lobbyler için lobbytemplate'den lobbycontainer içine instantiate ediyor. 
SetActive yapıyor ve LobbyListSingleUI'ına erişip .SetLobby(lobby) ile lobisini set ediyor. O da zaten ismini lobby.name ile alıyor. 
OnDestroy'da da lifetime farklarına korumaya almak için eventlerden unsub oluyoruz. 

Commitle ilgili yorumum:
 
****COMMIT 17 - 1c8251a
-Relay eklemek oldukça kolay. 
-KitchenGameLobby.cs'de hepsi try catch içinde ve hepsi async fonksiyonlar: 
Task<Allocation> AllocateRelay() fonksiyonu bir adet allocation döndürüyor. Bu arada async fonksiyonların tipi her zaman Task<T> oluyor. 
Çünkü Task klassı ayrıca taskın bitmesi durumu gibi konularda fonksiyonlara sahip, eventlere sahip. allocation bizim NetworkManager'ımızdaki Transportumuzu ayarlamak için kullanacağımız class denebilir.
Relay package'ından gelen RelayService'ın CreateAllocationAsync fonksiyonuyla MaxPlayer -1'lik bir allocation talebinde bulunulur.
-1 olma sebebi host'un sayısına gerek yok. Sonra bu allocation döndürülür. 
Task<String> GetRelayJoinCode(allocation allocation) Diğerleri bizim allocation'ımıza bağlansın diye kullanacakları kodu generate ediyor. 
RelayService'in GetJoinCodeAsync(allooation.allocationId) ile string döndürür. 
JoinRelayWithCode(string joinCode) da JoinAllocation'ı string kodu ile bulur ve joinAllocation'ı döndürür. 
CreateLobby fonksiyonu hostun kullanacaği fonksiyon, KitchenGameMultiplayer.cs'in .StartHost()'unu çağırmadan önce alokasyonu alıp kullanmamız lazım.
AllocateRelay() ile bir alokasyon alıyoruz. GetRelayJoinCode ile bu alokasyonun relay kodunu alıyoruz. 
///Daha sonra tekrar dönülecek.

Commitle ilgili yorumum: 


/WORK IN PROGRESS/