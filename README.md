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
Tam anlamadığım bir konu neden burningRecipe veya fryingRecipe'yi client tarafında da duyuruyoruz. State machine sadece serverda çalışıyor sonuçta.
??? 


Commitle ilgili yorumum: 

****COMMIT x - fasddas
-
Commitle ilgili yorumum: 
****COMMIT x- fasddas
-
Commitle ilgili yorumum: 
****COMMIT x - fasddas
-
Commitle ilgili yorumum: 


/WORK IN PROGRESS/