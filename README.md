# Projet API REST

**Projet réalisée par ROB Elioth**

La base de données du projet est hébergée sur le serveur OVH de Mr Luc Damas, il n'y a donc pas de configuration à effectuer, tout est géré dans le code. 

Version de Dotnet : **8.0.412**

Les dépendances à installer afin de pouvoir lancer le projet sont les suivantes : 


#### Dépendances Backend (API)
- AutoMapper — **12.0.1**
- AutoMapper.Extensions.Microsoft.DependencyInjection — **12.0.1**
- Microsoft.EntityFrameworkCore.InMemory — **8.0.20**
- Microsoft.EntityFrameworkCore.Tools — **8.0.4**
- Npgsql.EntityFrameworkCore.PostgreSQL — **8.0.4**
- Npgsql.EntityFrameworkCore.PostgreSQL.Design — **1.0.0**
- Swashbuckle.AspNetCore — **6.6.2**

####  Dépendances Tests
- bUnit — **1.40.0**
- JetBrains.Annotations — **2025.2.2**
- Microsoft.NET.Test.Sdk — **18.0.0**
- Microsoft.Playwright — **1.55.0**
- Microsoft.Playwright.NUnit — **1.55.0**
- Moq — **4.20.72**
- MSTest.TestAdapter — **3.11.0**
- MSTest.TestFramework — **3.11.0**
- Selenium.Support — **4.35.0**
- Selenium.WebDriver — **4.35.0**
- Selenium.WebDriver.ChromeDriver — **141.0.7390.5400**
- Selenium.WebDriver.GeckoDriver — **0.36.0**
- Selenium.WebDriver.MSEdgeDriver — **140.0.3485.66**


#### Dépendances Frontend (Blazor WebAssembly)
- Microsoft.AspNetCore.Components.WebAssembly — **8.0.18**
- Microsoft.AspNetCore.Components.WebAssembly.DevServer — **8.0.18**
- Microsoft.NET.ILLink.Tasks — **8.0.18**
- Microsoft.NET.Sdk.WebAssembly.Pack — **8.0.18**

Afin de les installer il faut se placer à la racine de la solution et executer la commande **dotnet restore**



**Lancement de l'application**

Lors du lancement du projet, un avertissement va apparaitre. Cliquer sur "Continue Anyway"

---

## Questions

### Quels principes SOLID le code de votre API REST respecte-t-il et lesquels ne respecte-t-il pas ?

#### Single Responsibility

La première règle SOLID est partiellement respectée dans ce projet. En effet, la plus grande partie des class possèdent un but propre. Que ce soit la communication, logique, accès aux données etc...

Cependant, le générique manager mélange par moment l'accès aux données et la logique (son but initial).

#### Open/Close

La structure de l'application permet d'ajouter facilement des éléments au projet sans avoir besoin de modifier ce dernier. L'héritage entre les class permet de créer de nouvelles catégories/fonctionnalités et d'implémenter le code déjà existant sans le modifier.

Exemples avec l'ajout d'une class Client :

- Après avoir écrit sa class EntityFrameworks et réalisé la migration il est possible de créer un manager client en héritant du GenericManager puis d'écrire le controller.

- Du côté application on peut faire hériter notre WSService du GenericWSService. Même chose pour le ViewModel avec un héritage de CrudViewModel. Si on veut ajouter des fonctions spécifiques à ces clients (connexion etc...) il est tout à fait possible de créer de nouveaux éléments d'application comme il est déjà le cas avec produit. (Dont les détails seront précisés plus bas dans ce document)

Aucun code n'est modifié durant ce processus → tout est ajouté.

#### Liskov Substitute

Ce principe est partiellement respecté dans le projet. Côté back toutes les class manager héritent de IDataRepository et implémentent grâce à GenericManager les méthodes dans le contrat. Ainsi partout où est appelé un IDataRepository<T>, les class qui en héritent peuvent forcément être utilisées comme tel sans que le comportement attendu soit cassé.

**Exceptions :**
- GetById peut renvoyer null → rupture du contrat
- GetByName (dont je reparlerai en détails plus bas) se doit d'avoir les types de class spécifiés afin de fonctionner. Si quelqu'un effectue un GetByString avec un IDataRepository<RandomEntity> le résultat sera forcément une erreur.

#### Interface segregation

Cette règle est également partiellement respectée durant ce projet. Le IDataRepository par exemple force certaines class à implémenter des fonctions dont elles n'ont pas l'intérêt (ex : marque et type produit n'ont pas besoin de getByName).

Cependant, certaines fonctionnalités sont séparées entre différentes interfaces (ex : IFiltrableRepository) mais ce n'est pas le cas pour tout comme nous le verrons.

#### Dependency injection

Chaque élément de haut niveau dans le backend utilise une abstraction des couches précédentes (ex : ProductController dépend de IFiltrableRepository<Product> et non directement de ProductManager). Toutes les dépendances sont injectées via le constructeur, il est donc simple de modifier la couche inférieure sans avoir besoin de modifier la couche actuelle.

De même pour le blazor, les éléments injectent leurs dépendances via le constructeur pour ne pas dépendre directement de la class inférieure mais bien de son abstraction.

---

### Si vous ne les avez pas appliqués dans votre code, quelles améliorations pourriez-vous mettre en place pour améliorer la qualité du code ?

#### Single responsibility

Je suppose qu'il faudrait ajouter différentes class qui vont chacune segmenter le travail du manager afin qu'il garde son unique responsabilité qui est de gérer la logique.

#### Liskov Substitute

Je ne sais pas comment faciliter cet accès aux données dans ces deux fonctions. Tout comme avec les autres méthodes il faudrait que ces dernières soient génériques à toutes les class.

#### Interface segregation

Il faudrait je pense séparer de nouveau les interfaces en plus petites comme l'annonce la règle. Par exemple une interface CRUD, une pour le get by name, et enfin une dernière pour les usages particuliers.

Cela rejoint la première règle. De ce fait les class n'ayant pas besoin de certaines méthodes n'auront pas besoin de les implémenter (ex : Marque et type produit n'ont pas besoin de get by name)

---

### Si vous avez appliqué des améliorations dans votre code, lesquelles sont-elles et que permettent-elles d'améliorer en termes de qualité ?

#### Ajouts à l'API

Afin de simplifier l'accès à un jeu de données depuis tous les posts qui download le projet sur github j'ai choisi d'héberger, comme dit en haut du document, la base de données sur le serveur de monsieur Damas.

Étant donné que des class sont liées entre elles (ex : Product ← Marque), j'ai ajouté une class EntityExtensions qui permet d'aller chercher les éléments de navigation lors d'un Get (ex : un produit est renvoyé avec ses éléments de navigation, NomMarque, NomTypeProduit).

J'ai choisi de faire des DTO uniquement pour la class Product. En effet, les deux autres class n'ont que très peu de propriétés, ainsi il n'est d'aucune utilité d'en créer pour eux. Leurs listes de navigations contenant les produits liés est vide car elles ne sont d'aucune utilité dans le front, ainsi il n'y a pas de transfert de données inutile.

Afin de faciliter le transfert entre les objets et leurs DTO j'ai utilisé AutoMapper, il n'y a donc pas besoin de coder chaque transfert de propriété. La seule chose à ajouter est les navigations entre les class. Les objets avant de se faire convertir possèdent leurs navigations grâce à EntityExtensions (ex: product possède un objet Marque), puis avec cet ajout de code le NomMarque est ajouté au DTO.

L'utilisation de class génériques comme GenericManager demande par moment des éléments spécifiques des class, par exemple dans un GetById il faut pouvoir avoir accès à l'ID. L'objet étant générique il n'est pas possible de récupérer son ID. J'ai donc fait hériter d'une interface IEntity à mes Entities. Cette interface implémente une fonction qui permet de récupérer l'Id de l'objet et une autre fonction permet de récupérer son Nom. Ainsi il sera possible dans le GenericManager de récupérer l'id de l'objet car il est défini que ce dernier est un IEntity. (la même logique est utilisée dans le Blazor et pour les DTO)

Afin de générer les managers il existe plusieurs couches :

![Manager schemas](Images/Manager.png)

IDataRepository comprend dans son contrat les fonctions basiques (get, getall, delete, put, post) et se charge donc de la relation avec les données.

IFiltrableRepository lui permet d'implémenter la fonction de filtre (utilisée par produit), une seule class étant concernée j'ai décidé de mettre cette interface à part. L'ajouter dans le IDataRepository forcerait à implémenter la fonction de filtre dans les autres managers alors qu'ils ne peuvent pas → Liskov substitute principle

Le GenericManager se charge simplement d'implémenter les méthodes imposées par IDataRepository de manière générique afin de générer par la suite TypeProduitManager et MarqueManager.

ProductManager lui hérite donc de GenericManager et de IFiltrableRepository, GenericManager lui implémente les fonctions de base et IFiltrableRepository lui impose d'implémenter la méthode de filtre.

#### Ajouts à Blazor

Une class NotificationService permet de faire des notifications en haut à droite de l'écran, cela est utilisé lorsqu'une marque est ajoutée, modifiée, supprimée etc...

Un attribut IgnoreInTemplateAttribute est utilisé dans le code des class. En effet, tous les forms de l'application (modification ou ajout d'éléments) parcourent la class concernée propriété par propriété afin de générer les inputs adaptés. Ainsi si des champs sont ajoutés à ces class il n'y aura pas besoin de retoucher les forms. Le IgnoreInTemplate permet d'ignorer dans ces forms certaines propriétés comme l'Id ou les champs de navigation où l'on préférera utiliser un select (ex : un produit ne peut qu'être lié avec des marques existantes).

Les services de l'application sont générés de manière générique. Une interface permet d'imposer les méthodes à implémenter, un GenericWSService implémente ces dernières peu importe la class et enfin les différents services héritent de ce GenericWSService.

![WSService schemas](Images/WSService.png)

Le service de produit implémente la méthode de filtre, je pense que j'aurais dû implémenter cette méthode via une interface au lieu de le faire de manière brute.

Les class Marque et Type Produit sont très similaires dans leurs champs ainsi que dans leurs usages. Ainsi afin de ne pas répéter de code j'ai développé beaucoup d'éléments génériques propres aux deux.

Exemple avec les ViewModels :

![ViewModel schemas](Images/ViewModel.png)

Les logiques des deux pages qui géreront Marque et Type Produit étant strictement les mêmes j'ai donc choisi de structurer mes ViewModels ainsi. ICrudViewModel impose les méthodes. CrudViewModel les implémente. BaseEntityViewModel (nom à changer) lui va s'occuper de gérer la logique commune à Marque et Type Produit.

ProductViewModel lui hérite simplement de CrudViewModel.

Les pages de ces deux class utilisent le même template complètement générique (TableTemplate). Si une des deux class se trouve être modifiée, le fonctionnement restera le même, il n'y aura pas besoin de modifier le code de ces pages.

Encore une fois les class sont parcourues champs par champs afin de tous les afficher.

IStateService implémente les trois class de notre projet, Cela permet dans les Forms d'ajout d'éditer directement un objet avant de le post. Cela permet par exemple de changer de page et de revenir sur cette dernière sans perdre les informations indiquées. Le form d'ajout parcourt également chaque champ de la class concernée.

#### Tests

Afin de ne pas endommager le jeu de données dans la base de données existante, j'ai implémenté à l'aide de EF Core InMemory une simulation de la base basée sur le DbContext.

Il n'y a donc pas de réelle base de données de tests mais elle se base bien sur celle existante.

Afin de ne pas me répéter dans l'écriture de mes tests j'ai utilisé un TestInitialize qui permet de créer des objets utilisables par n'importe quelle méthode de test dans le fichier. J'ai également ajouté un Cleanup à la fin de chaque test afin de ne pas être influencé par les précédents.

#### Points à améliorer dans le projet

En plus de tous les points déjà cités durant ce document il reste ces points-ci à améliorer à mon sens :

- Corriger les erreurs potentielles au niveau des héritages entre les managers ou les ViewModels (je ne sais pas si c'était la bonne façon de faire).

Les points suivants n'ont pas été implémentés par manque de temps :

- L'interface graphique du site est à refaire complètement.

- Ajouter des vérifications dans les forms : en modifiant la class NotificationService il est assez simple d'afficher des messages customisés en cas d'erreur de saisie et même en cas d'erreur lors d'actions des ViewModels. Double validation avant une suppression.

  - Ajouter un controller de médias : Ma version de Blazor ne me permet pas de stocker des images directement dans le projet Application. Il faudrait donc créer un MediaController ainsi qu'un MediaManager côté API (et les mêmes côté application) afin de pouvoir stocker des images au lieu de donner un lien internet.

    Exemple d'un ancien projet d'une méthode de controller pour récupérer une image :

          [HttpGet("Photos/{fileName}")]
            public IActionResult GetPhotos(string fileName)
            {
            var path = _mediaManager.GetMediaPath("Photos",  fileName);

              if (path == null)
              {
                  return NotFound();
              }
              return PhysicalFile(path, "image/jpeg");
          }

- Améliorer les tests E2E : Je trouve mes tests E2E très légers. (je n'ai pas eu le temps de me renseigner assez afin de les améliorer). De plus j'ai rencontré de nombreuses erreurs avec ces derniers, m'imposant l'utilisation de Task.Delay()

- Le code n'est pas entièrement en Anglais, il reste de nombreux mots en Français dans le code.

- Les fonctions du GenericManager doivent être corrigées afin de rendre ce dernier complètement extensible à l'ajout de nouveaux managers héritant de ce dernier sans devoir le modifier.

- ProductWSService : Peut-être ajouter une interface qui impose l'implémentation de la méthode de filtre au productWSService (je ne sais pas si c'est la bonne façon de faire).

---

## Les fonctionnalités du site

### Page des produits

Il est possible de visualiser tous les produits sur cette page ainsi que de les filtrer à l'aide des sélecteurs de marque et de type produit. Il est aussi possible de rechercher par mot clé. Tous ces filtres sont parfaitement combinables.

En cliquant sur le bouton "Add a product" on se rend sur la page de création de produit.

En cliquant sur un produit il est possible de se rendre sur la page détails de ce dernier.

### Page de création de produit

Cette page permet de remplir le form de création d'un produit. Après avoir rempli les champs, cliquer sur Add permet d'ajouter le produit.

### Page de détails d'un produit

Cette page, similaire à la précédente permet de visualiser un form contenant toutes les informations d'un produit. Il est possible de les modifier en changeant les champs puis en appuyant sur Update. Il est aussi possible de supprimer le produit en appuyant sur Delete.

### Page de Marque et Type Produit

Ces deux pages permettent de sélectionner un élément puis de le modifier ou encore de le supprimer. Un form sur la droite permet aussi la création d'un nouvel élément.