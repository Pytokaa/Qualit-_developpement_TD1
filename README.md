API réalisée par ROB Elioth



Questions : 

 - **Quels principes SOLID le code de votre API REST respecte-t-il et lesquels ne
   respecte-t-il pas ?**


    Single Responsibility : 

    La première règle SOLID est partiellement respectée dans ce projet. En effet, la plus grande partie des class possèdent un but propre. Que ca soit la communication, logique, accès aux données etc.. 
    Cependant, le générique manager mélange par moment l'accès aux données et la logique (sont but inital). Le productController lui aussi regroupe beaucoup de fonctionnalités differentes et enfreint cette première règle. 



    Open/Close : 

    La structure de l'application permet d'ajouter facilement des éléments au projet sans avoir besoin de modifier ce dernier. L'héritage entre les class permet de créer de nouvelles catégories/fonctionnalités et d'implémenter le code déjà existant sans le modifier. 
    Exemples avec l'ajout d'une class Client: 

        - Après avoir écrit sa class EntityFrameworks et réalisé la migration il est possible de créer un manager client en heritant du GenericManager puis d'écrire le controller. 
        
        - Du coté application on peut faire hériter notre WSService du GenericWSService. Même chose pour le ViewModel avec un héritage de CrudViewModel. Si on veut ajouter des fonctions spécifiques à ces clients (connexion etc...) il est tout a fait possible de créer de nouveaux éléments d'application comme il est déjà le cas avec produit. (Dont les détails seront précisés plus bas dans ce document) 
        Aucun code n'est modifié durant ce processus -> tout est ajouté. 

    
    Liskov Substitute : 

    Ce principe est partiellement respecté dans le projet. Coté back toutes les class manager héritent de IDataRepository et implémentent grace à GenericManager les methods dans le contrat. Ainsi partout ou est appelé un IDataRepository<T>, les class qui en héritent peuvent forcément être utitisé comme tel sans que le comportement attendu soit cassé. 
    Exceptions :    
        - GetById peut renvoyer null -> rupture du contrat
        - GetByName (dont je reparlerai en détails plus bas) se doit d'avoir les type de class spécifié afin de fonctionner. Si quelqu'un effectue un GetByString avec un IDataRepository<RandomEntity> le resultat sera forcément une erreur. 



    Interface segregation : 
    
    Cette règle est également partiellement respecté durant ce projet. Le IDataRepository par exemple force certaines class à implementer des fonctions dont elles n'ont pas l'interet (ex : marque et type produit n'ont pas besoin de getByName). 
    Cependant, certaines interfaces permettent 
    

    
    




 - **Si vous ne les avez pas appliqués dans votre code, quelles améliorations
  pourriez-vous mettre en place pour améliorer la qualité du code ?**




 - **Si vous avez appliqué des améliorations dans votre code, lesquelles
  sont-elles et que permettent-elles d’améliorer en termes de qualité ?**

 
    API : 

    L'utilisation de AutoMapper permet de transiter entre un objet et son DTO très facilement sans avoir besoin de modifier du code. Tout est automatique. 

    La logique de l'api se trouve dans les managers. Afin de ne pas repeter du code et de centraliser ce dernier, une abstract class GenericManager existe. La gestion générique des entités permet de facilement créer de nouveaux manager. 

    Il est possible de filtrer les produits a l'aide d'une fonction qui est ajoutée par l'interface IFiltrableRepository. Cette interface est ajouté au manager de produit. Ne sachant pas la bonne pratique je voyais plusieurs manières de faire : 

        - Ajouter la méthod dans le IDataRepository, ne pas ajouter de logique dans le générique et l'override dans le produit manager -> probleme : non respect des règles SOLID, les autres manager heritent d'une méthod qu'ils ne peuvent utiliser

        - Faire hériter IdataRepository à IFiltrableRepository afin d'avoir une seule interface qui annonce les methods -> probleme : le manager étant générique, il est difficile de s'adapter a différents types de Repository (ca ne serait pas SOLID)

        - Créer deux manager, le produitManager qui permet de faire les commandes classiques + un manager de filtre. -> problème : a mon sens pas très propre d'implémenter deux manager dans un controller.

        - Lors de la création du manager produit, ajouter un héritage à l'interface IFiltrable afin de créer un manager adapté -> solution que j'ai choisi 

    Blazor : 

    Même logique que pour les managers, une abstract class permet de simplement ajouter des services en héritant de cette dernière. Cela évite de  nouveau la répétition de code.

    Afin de ne pas devoir réécrire le code en cas de modification des class, tous les form présents dans l'application sont générés en parcourant les entités. Les champs qui ne doivent pas apparaitre sont indiqué a l'aide du tag "IgnoreInTemplate"


**Plus de détails sur le code :**

    API : 
    
    Le 


    Blazor : 

    (Compontents/Layout/TableTemplate.razor)
    Les pages /brands et /productTypes sont générée par le meme template. Tout est automatisé de facon générique. Ainsi si une des class est modifiée il n'y aura pas besoin de recoder la page. 
    L'utilisation de ce template a été choisi en raison du fait que les deux class sont très similaire dans leur utilité. Les deux n'ont pas vocation a posseder beaucoup de propriétés et possèdent les mêmes usages (visualiser, modifier, supprimer, ajouter). 
    Ce template permet donc de ne pas se repeter. Et si jamais une nouvelle class du même type doit etre ajouté cela sera facilement faisable.
    Afin de garantir le fonctionnement de ce template, il est définie que les entités du template héritent forcement d'une interface IEntity qui implemente deux fonctions : une pour récuperer le nom de l'entity et l'autre pour récuperer son ID. 

    La class produit elle est gérée par un fichier razor dédié en raison des éléments plus complexes ajouté (trie, recherche par mot clée, visualisation d'image etc..). L'affichage de ces derniers étant très spécifique, une page lui est dédiée directement.   
    