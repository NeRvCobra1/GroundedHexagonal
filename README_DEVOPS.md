# GitHub Actions DevOps starter

Copy `.github/` to the repository root.

## CI
`dotnet-ci.yml` runs on pull requests to `main`, pushes to `main`, and manually.

It:
- installs .NET 10
- restores local dotnet tools
- restores/builds the main solution
- runs the automated test suite
- checks EF Core pending model changes
- restores/builds the standalone Visualizer
- uploads `.trx` test results

## Continuous Delivery
`dotnet-release.yml` runs on version tags like `v1.0.0`.

It:
- rebuilds and retests
- verifies EF migrations
- publishes API, Worker and Visualizer
- packages them as ZIPs
- uploads workflow artifacts
- creates a GitHub Release for tag-triggered runs

This is Continuous Delivery, not deployment to a live server. A later deployment
workflow can target Azure, Google Cloud Run, a VM, containers, etc.
