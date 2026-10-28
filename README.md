# CleanArchitecture

API REST e-commerce en .NET 10, organisée en Clean Architecture 

## Structure

- `CleanArchitecture.Domain` : entités, objets-valeurs, règles métier et exceptions.
- `CleanArchitecture.Application` : DTOs, interfaces de dépôt et services applicatifs.
- `CleanArchitecture.Infrastructure` : dépôts EF Core et persistance SQLite.
- `CleanArchitecture.Api` : contrôleurs REST et configuration de l'API.

Les contrôleurs appellent directement les services applicatifs. Le contexte SQLite est créé au démarrage avec `EnsureCreated`; le fichier `cleanarchitecture.db` est stocké dans le répertoire de travail du processus API.

## Démarrer

```powershell
dotnet build CleanArchitecture.slnx
dotnet run --project src\CleanArchitecture.Api\CleanArchitecture.Api.csproj
```

En environnement de développement, le document OpenAPI est disponible à `/openapi/v1.json`.

## Routes

| Méthode | Route | Opération |
|---|---|---|
| GET, POST | `/api/categories` | Lister et créer des catégories |
| GET, PUT, DELETE | `/api/categories/{id}` | Lire, modifier et supprimer une catégorie |
| GET, POST | `/api/products` | Lister et créer des produits |
| GET, PUT, DELETE | `/api/products/{id}` | Lire, modifier et supprimer un produit |
| GET, POST | `/api/customers` | Lister et créer des clients |
| GET, PUT, DELETE | `/api/customers/{id}` | Lire, modifier et supprimer un client |
| GET, POST | `/api/orders` | Lister et créer des commandes |
| GET | `/api/orders/{id}` | Lire une commande |

Les produits nécessitent une catégorie. Une commande nécessite un client et des produits existants; sa création vérifie et met à jour le stock et calcule le total. Les montants utilisent une devise à trois lettres (par exemple `EUR`).

### Exemple de création de commande

```json
{
  "customerId": "GUID_DU_CLIENT",
  "shippingAddress": {
    "street": "1 rue de Casablanca",
    "city": "Rabat",
    "postalCode": "10000",
    "country": "Maroc"
  },
  "items": [
    {
      "productId": "GUID_DU_PRODUIT",
      "quantity": 2
    }
  ]
}
```
