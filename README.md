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