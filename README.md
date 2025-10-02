API réalisée par ROB Elioth



Questions : 

 - **Quels principes SOLID le code de votre API REST respecte-t-il et lesquels ne
   respecte-t-il pas ?**


    Réspectés : 

    Single responsibility :  Chaque élément possède une seule utilité. 
    Le controller, lui, sert à faire la jonction entre les requêtes HTTP.
    Le manager lui va s'occuper de la partie logique de l'application. 
    Le repository se charge de faire la jonction entre la base de données et les objects EntityFramework. 

    Dependency inversion :  la logique de l'api est définie par l'interface IDataRepository. 
    Le constructeur du controller implémente un IdataRepository<Entity> et non un ProductManager concret, ainsi il peut utiliser les méthods définies dans ce derniers
    sans dépendre de son fonctionnement. La logique d'un manageur peut être modifiée sans impacter le controller.
    Nous retrouvons un fonctionnement similaire avec les DTO, l'utilisation de AutoMapper permet de passer facilement entre un objet et son DTO sans dépendre de ses propriétés.

    Open/close principle :  Il existe une version abstract générique de la logique du manager. Il est donc facile d'ajouter des extensions en ajoutant un nouveau manager qui herite 
    du générique, mais aussi d'ajouter de nouvelles fonctionnalités comme avec le Filtre pour les produits. 


    Non réspectés : 

    
    




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
    