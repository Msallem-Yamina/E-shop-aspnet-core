# E-Shop

Boutique en ligne **ASP.NET Core MVC (.NET 8)** : catalogue d'articles, panier, commandes et tableau de bord, avec accès aux données via **Entity Framework Core (approche Code First)** et gestion des utilisateurs avec **ASP.NET Core Identity**.

## Fonctionnalités

### Gestion des articles et catégories

- CRUD complet des **catégories** (liste, création, modification, détails, suppression)
- CRUD complet des **articles** (liste en cartes, création, modification, détails, suppression)
- **Upload d'image** pour chaque article (stockée dans `wwwroot/images`)
- **Recherche** par nom d'article ou par nom de catégorie (barre de navigation)
- Relation **Category 1 - N Product** (liste déroulante de catégories dans les formulaires)
- Validation des champs (DataAnnotations)
- Interface responsive avec le thème Bootswatch **Flatly**, cartes cliquables, badges de stock
- **Page d'accueil** : articles paginés et filtre par catégorie

### Authentification et autorisation

- **Inscription, connexion et déconnexion** (Register, Login, Logout)
- Menu dynamique : *Register / Login* ou menu du compte selon l'état de connexion
- **Gestion des rôles** (Admin, Manager, User) : créer, modifier, supprimer, avec confirmation avant suppression
- **Affectation des utilisateurs aux rôles** (interrupteurs)
- **Autorisation par rôle** avec `[Authorize]` et `[AllowAnonymous]`
- Page **Accès refusé**
- **Sidebar** affiché une fois connecté, avec un contenu selon le rôle (absent de la page d'accueil)
- Interface Login / Register modernisée (labels flottants, icône œil pour afficher le mot de passe)

### Panier, commandes et tableau de bord

- **Panier** : ajout, modification des quantités, suppression et compteur dans la barre de navigation
- **Commande** : validation du panier, page de confirmation et historique *Mes commandes*
- **Tableau de bord Admin** : statistiques sur les articles, les catégories, les utilisateurs, les commandes et le stock
- Tableau de bord simplifié pour les autres utilisateurs connectés

## Droits par profil

| Profil | Catalogue et panier | Commander, tableau de bord | Gérer les catégories | Gérer les articles | Administration (rôles) |
|---|:---:|:---:|:---:|:---:|:---:|
| Visiteur (non connecté) | Oui | Non (redirigé vers Login) | Non (redirigé vers Login) | Non (redirigé vers Login) | Non |
| User | Oui | Oui | Oui | Non (Accès refusé) | Non |
| Manager | Oui | Oui | Oui | Oui | Non |
| Admin | Oui | Oui | Oui | Oui | Oui |

## Technologies

| Élément | Technologie |
|---|---|
| Framework | ASP.NET Core MVC (.NET 8) |
| ORM | Entity Framework Core 8 (Code First, migrations) |
| Authentification | ASP.NET Core Identity (`Microsoft.AspNetCore.Identity.EntityFrameworkCore`) |
| Base de données | SQLite (fichier `MyBaseDB.db`) |
| Front | Bootstrap 5 (thème Bootswatch Flatly), Bootstrap Icons, Razor |
| Patterns | Repository, Injection de dépendances, ViewModels |

> Le projet utilise SQLite. Pour passer à SQL Server, le code EF Core reste identique : seul le fournisseur change (`UseSqlServer` au lieu de `UseSqlite`).

## Structure du projet

```
TP_Product/
├── Controllers/        HomeController (accueil), ProductController, CategoryController,
│                       PanierController (panier, commandes), DashboardController,
│                       AccountController (Register, Login, Logout, AccessDenied),
│                       AdminController (gestion des rôles)
├── Models/
│   ├── Product.cs, Category.cs, Order.cs, OrderItem.cs, AppDbContext.cs (IdentityDbContext)
│   ├── RegisterViewModel.cs
│   └── Repositories/   IProductRepository, ICategorieRepository, IOrderRepository + implémentations
├── ViewModels/         CreateViewModel, EditViewModel, LoginViewModel, OrderViewModel,
│                       CartItemViewModel, AdminDashboardViewModel,
│                       CreateRoleViewModel, EditRoleViewModel, UserRoleViewModel
├── Views/              Home/, Product/, Category/, Panier/, Dashboard/, Account/, Admin/, Shared/
├── Migrations/         Migrations EF Core (dont MajIdentity et Order_Migration)
├── wwwroot/            css, js, images
└── Program.cs          Configuration, injection de dépendances, Identity
```

## Prérequis

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- Outil EF Core (pour les migrations) :

```bash
dotnet tool install --global dotnet-ef --version 8.0.*
```

## Lancer le projet

```bash
git clone https://github.com/Msallem-Yamina/Gestion-Articles.git
cd Gestion-Articles
dotnet restore
dotnet ef database update
dotnet run
```

Puis ouvrir l'adresse affichée dans le terminal (par exemple `http://localhost:5062`).

Pour le rechargement automatique pendant le développement : `dotnet watch run`.

## Base de données et migrations

La chaîne de connexion se trouve dans `appsettings.json` :

```json
"ConnectionStrings": {
  "ProductDBConnection": "Data Source=MyBaseDB.db"
}
```

Commandes de migration (console NuGet de Visual Studio ou terminal) :

| Visual Studio | Terminal |
|---|---|
| `Add-Migration InitialCreate` | `dotnet ef migrations add InitialCreate` |
| `Add-Migration MajIdentity` | `dotnet ef migrations add MajIdentity` |
| `Add-Migration Order_Migration` | `dotnet ef migrations add Order_Migration` |
| `Update-Database` | `dotnet ef database update` |

## Créer le premier administrateur

Le rôle Admin est nécessaire pour accéder à l'administration, mais il faut être Admin pour créer des rôles. Pour amorcer :

1. Lancer l'application et créer un compte via **Register**.
2. Dans `AdminController`, mettre temporairement en commentaire `[Authorize(Roles = "Admin")]`.
3. Ouvrir `/Admin/CreateRole` et créer les rôles **Admin**, **Manager** et **User**.
4. Dans la liste des rôles : *Modifier* le rôle Admin, puis *Ajouter ou retirer des utilisateurs*, et s'attribuer Admin.
5. Remettre l'attribut `[Authorize(Roles = "Admin")]`, relancer l'application, puis **Logout / Login** (les rôles sont lus à la connexion).

## Utilisation

1. Parcourir les articles sur la page d'accueil et les filtrer par catégorie.
2. Créer des **catégories** depuis le sidebar (*Nouvelle catégorie*).
3. Créer des **articles** avec une image depuis le sidebar (*Nouvel article*, Admin ou Manager).
4. Cliquer sur une carte pour voir les **détails** ; Admin et Manager modifient ou suppriment avec les icônes de la carte.
5. Utiliser la barre de **recherche** pour filtrer par nom d'article ou de catégorie.
6. Ajouter des articles au **panier**, valider la commande, puis la retrouver dans *Mes commandes*.
7. Consulter les statistiques dans le **Tableau de bord** et gérer les rôles depuis la section **Administration** du sidebar (Admin uniquement).

## Auteur

**Yamina Msallem**
