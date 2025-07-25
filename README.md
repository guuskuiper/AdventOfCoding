# How to run

```dotnet run ```

to run the latest solution. Or pass a day number to select a specific solution.

```dotnet run -- 5```

to run the solution for the 5th of December.

## Automation

Automatically download the input using the `AoCClient` in `src\AdventOfCode.Client`.

The client requires a session key.
Either create a `SESSION` environment variable or a file `SESSION` in `src\AdventOfCode.Client`.
The content should start with: `session=` and after that, the remaining content should be 128 characters.

The client can also download and parse private leaderboard statistics.  

- Outbound calls are throttled to every 15 minutes in (`GetLeaderboard()`)
- Once inputs are downloaded, they are cached locally (`RequestOrCacheAsync()`)
- The User-Agent header in `AoCClient` is set to this Github repository, where issues can be added.

## CI/CD

The Actions uses a secret environment variable to access the personal input's. The Personal Access Token (PAT) does expire.

Create / refresh the PAT on https://github.com/settings/personal-access-tokens for [the repository](https://github.com/guuskuiper/AdventOfCoding-Input).

Set the Token in the CI_SECRET: https://github.com/guuskuiper/AdventOfCoding/settings/secrets/actions
