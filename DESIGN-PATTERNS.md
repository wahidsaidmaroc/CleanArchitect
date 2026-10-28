# Patterns et principes appliqués

Ce document décrit les patterns et principes présents dans l'application.
Elle utilise des services applicatifs appelés directement par l'API, sans
CQRS ni MediatR.

## Clean Architecture

Clean Architecture répartit les responsabilités en couches et garde les règles
métier au centre. Le domaine ne dépend ni de l'API ni d'EF Core. Les détails de
persistance sont dans Infrastructure.

C'est un style d'architecture, pas un pattern GoF isolé.

## Dependency Inversion Principle (DIP)

Les services applicatifs dépendent d'abstractions comme `IRepository<TEntity>`
et `IUnitOfWork`, déclarées dans Application. Infrastructure fournit les
implémentations EF Core correspondantes.

```text
OrderService ──► IUnitOfWork ◄── EfUnitOfWork
                               (Infrastructure)
```

Application n'a donc pas besoin de connaître `AppDbContext`, EF Core ou SQLite.
Le principe d'inversion des dépendances est le « D » de SOLID.

## Repository

`IRepository<TEntity>` décrit des opérations communes pour lister, chercher,
ajouter et supprimer une entité. `EfRepository<TEntity>` les réalise avec EF
Core.

```text
Service applicatif
        │ utilise
        ▼
IRepository<Product> ◄── EfRepository<Product> ──► AppDbContext
       Application               Infrastructure
```

Le service consomme le contrat plutôt que de communiquer directement avec le
contexte de base de données.

## Unit of Work

`IUnitOfWork` expose `SaveChangesAsync`, et `EfUnitOfWork` délègue cet appel à
`AppDbContext`. L'interface est dans Application pour que les services puissent
demander la sauvegarde sans dépendre d'Infrastructure; son implémentation est
dans Infrastructure.

Lors d'une commande, le contexte suit les modifications apportées aux produits
et à la nouvelle commande. L'appel à `SaveChangesAsync` les persiste ensemble.
EF Core exécute normalement un `SaveChanges` dans une transaction; il ne s'agit
pas ici d'une transaction distribuée.

## Service Layer

`CustomerService`, `CategoryService`, `ProductService` et `OrderService`
coordonnent les opérations de l'application. Ils chargent les entités,
orchestrent les règles métier via le domaine, demandent leur persistance puis
construisent la réponse DTO.

Les contrôleurs appellent directement ces services. Le projet ne contient pas
de `Command`, `Query`, `Handler` ou pipeline MediatR.

## Entity et Value Object

- **Entity** : objet possédant une identité. `Customer`, `Category`, `Product`
  et `Order` héritent de `BaseEntity`.
- **Value Object** : objet qui représente une valeur métier plutôt qu'une
  identité indépendante. `Address` et `Money` valident leurs données et ne
  rendent pas leurs propriétés modifiables publiquement.

EF Core les configure comme objets possédés; leur durée de vie dans le modèle
est liée à leur entité propriétaire.

## DTO et mapping

Les DTOs (`CustomerRequest`, `ProductResponse`, `OrderRequest`, etc.) définissent
les données échangées par l'API. Les entités du domaine ne sont donc pas
retournées directement au client HTTP.

Le helper `Mapping` convertit les entités en réponses DTO et les adresses
entrantes en objets-valeurs du domaine.

## Injection de dépendances

Le conteneur .NET construit et fournit les contrôleurs, services et dépôts.
L'API sert de point de composition : elle enregistre les interfaces avec leurs
implémentations concrètes. Les dépendances sont reçues par constructeur plutôt
que créées à la main dans les classes.

## Résumé

| Élément | Rôle dans le projet |
|---|---|
| Clean Architecture | Organiser les couches et les dépendances |
| DIP | Faire dépendre Application d'abstractions |
| Repository | Abstraire l'accès aux entités persistées |
| Unit of Work | Exposer l'enregistrement des changements |
| Service Layer | Orchestrer les cas d'utilisation applicatifs |
| Entity | Représenter un objet métier avec une identité |
| Value Object | Encapsuler une valeur métier validée |
| DTO | Définir les échanges HTTP sans exposer les entités |
| Injection de dépendances | Assembler les implémentations au démarrage |

## Voir aussi

- [Architecture et relations entre les classes](README-ARCHITECTURE.md)
