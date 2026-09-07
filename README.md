# VideoGamesLibrary — API de démonstration

API REST de gestion d'une ludothèque, construite pour illustrer la **Clean Architecture**
et le fonctionnement d'une API en C# / ASP.NET Core.

## Stack

- .NET 10 / ASP.NET Core (controllers)
- Entity Framework Core 10 + SQLite
- Authentification JWT (Bearer)
- Swagger / Swashbuckle
- xUnit (tests unitaires + tests d'intégration)

## Architecture

Quatre couches, avec une règle unique : **les dépendances pointent toujours vers l'intérieur**.

| Projet | Rôle | Dépend de |
|---|---|---|
| `VideoGamesLibrary.Domain` | Entités (`Game`, `User`) et interfaces de repositories | *rien* |
| `VideoGamesLibrary.Application` | DTOs, services métier, interfaces (`IPasswordHasher`, `ITokenGenerator`) | Domain |
| `VideoGamesLibrary.Infrastructure` | EF Core, repositories, migrations, JWT, hachage | Application, Domain |
| `VideoGamesLibrary.Api` | Controllers, injection de dépendances, Swagger | Application, Infrastructure |

`Domain` et `Application` n'ont **aucun package NuGet externe** : c'est ce qui permet de tester
le métier sans base de données ni serveur web.

Les interfaces `IPasswordHasher` et `ITokenGenerator` sont déclarées dans `Application`
mais implémentées dans `Infrastructure` : c'est l'inversion de dépendance qui permet à la
couche métier d'utiliser du JWT sans jamais en dépendre.

## Prérequis

- [SDK .NET 10](https://dotnet.microsoft.com/download)
- L'outil EF Core en ligne de commande :

```bash
dotnet tool install --global dotnet-ef
```

## Démarrage

Toutes les commandes se lancent depuis le dossier `VideoGamesLibrary`.

### 1. Créer la base de données (obligatoire au premier lancement)

La base SQLite n'est pas versionnée. Il faut donc la créer **avant** de démarrer l'API,
sinon le démarrage échoue avec `SQLite Error 1: 'no such table: Users'`.

```bash
dotnet ef database update -p VideoGamesLibrary.Infrastructure -s VideoGamesLibrary.Api
```

Cette commande crée `VideoGamesLibrary.Api/VideoGamesLibrary.db` en appliquant les migrations.

### 2. Lancer l'API

```bash
dotnet run --project VideoGamesLibrary.Api
```

L'API démarre sur **http://localhost:5104**. Swagger est servi **à la racine** :
ouvrir <http://localhost:5104> affiche directement l'interface.

Au premier démarrage, les données de démonstration (3 utilisateurs, 2 jeux) sont insérées
automatiquement par `DbInitializer`.

### Repartir d'une base vierge

Supprimer les fichiers de base puis rejouer l'étape 1 :

```bash
rm VideoGamesLibrary.Api/VideoGamesLibrary.db*
```

## Comptes de démonstration

| Utilisateur | Mot de passe | Rôle |
|---|---|---|
| `admin` | `admin` | Admin |
| `user` | `user` | User |
| `plop` | `plop` | User |

Les mots de passe sont stockés hachés (PBKDF2 via `PasswordHasher` d'ASP.NET Core Identity).

## Endpoints

| Méthode | Route | Authentification | Réponses |
|---|---|---|---|
| `POST` | `/api/User/login` | anonyme | 200, 400, 401 |
| `GET` | `/api/Games` | JWT requis | 200, 401 |
| `GET` | `/api/Games/{id}` | JWT requis | 200, 401, 404 |
| `POST` | `/api/Games` | JWT requis | 201, 400, 401 |
| `PUT` | `/api/Games/{id}` | JWT requis | 204, 400, 401, 404 |
| `DELETE` | `/api/Games/{id}` | JWT requis | 204, 401, 404 |

## Tester l'API

**Depuis Swagger** : se connecter via `POST /api/User/login`, copier le `token` de la réponse,
puis cliquer sur **Authorize** et coller le jeton.

**Depuis Visual Studio** : ouvrir `VideoGamesLibrary.Api/VideoGamesLibrary.Api.http`.
Les 9 requêtes couvrent le parcours complet (login, 401, CRUD, 400, 404) et réutilisent
automatiquement le jeton de la première requête.

## Lancer les tests

```bash
dotnet test
```

- `VideoGamesLibrary.UnitTests` — teste `GameService` avec un faux repository en mémoire,
  sans base ni serveur.
- `VideoGamesLibrary.Api.IntegrationTests` — démarre l'API entière en mémoire avec une base
  SQLite `:memory:`, et rejoue un vrai appel HTTP authentifié.

Les tests n'utilisent jamais `VideoGamesLibrary.db` : ils sont isolés de la base de développement.

## Docker

```bash
docker compose up --build
```

L'API est alors disponible sur **http://localhost:6400**. En environnement `Docker`, les
migrations sont appliquées automatiquement au démarrage (le conteneur part d'une base vide).

## Commandes EF Core utiles

```bash
dotnet ef migrations add <NomDeLaMigration> -p VideoGamesLibrary.Infrastructure -s VideoGamesLibrary.Api -o Migrations
dotnet ef database update -p VideoGamesLibrary.Infrastructure -s VideoGamesLibrary.Api
```

`-p` désigne le projet qui contient le `DbContext` et les migrations,
`-s` le projet de démarrage qui porte la configuration (chaîne de connexion).

## TP

- **Création du CRUD d'un utilisateur** : Aujourd'hui, on gère les jeux et l'authentification.
Il faut donc ajouter la partie des utilisateurs.
Cela nécessitera de repasser par toutes les couches pour voir les modes de communications.
Il ne faudra pas oublier lors de la création d'un utilisateur d'utiliser la notion de hashage de mot de passe.
