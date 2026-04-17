# LethalCompanyMod Constitution

## Core Principles

### I. Code Quality Standards

Code MUST be readable, maintainable, and follow consistent formatting. All code changes MUST pass linting and type checking before merging. Contributors SHOULD refactor code that violates the principle of least surprise—behavior should match reasonable expectations. Complexity MUST be justified with comments explaining why simpler alternatives were rejected.

### II. Testing Requirements

Every feature MUST have corresponding tests. Unit tests MUST cover core logic paths, and integration tests MUST verify mod compatibility with the base game. Tests MUST be runnable in isolation and MUST NOT depend on external services or game state. Test coverage for critical paths (saving, loading, networking) MUST exceed 80%.

### III. Security Practices

Code MUST NOT expose sensitive player data or allow unauthorized game state manipulation. All configuration values MUST be validated at runtime. External dependencies MUST be audited for known vulnerabilities before inclusion. Network communications MUST use appropriate encryption where the base game supports it.

### IV. Maintainability Guidelines

Modules MUST be loosely coupled with clear interfaces. State management MUST be centralized and predictable. Documentation MUST explain the "why" behind non-obvious design decisions. Breaking changes MUST be versioned appropriately and include migration guidance.

### V. Observability & Debugging

Mod behavior MUST be traceable via logging at appropriate verbosity levels. Error conditions MUST produce actionable log messages with sufficient context. Debug builds MUST expose diagnostic tools; release builds MUST disable debug endpoints.

## Security Requirements

All user inputs MUST be sanitized before processing. File system operations MUST validate paths and use sandbox-appropriate permissions. Configuration loading MUST handle missing/corrupt files gracefully. Memory management MUST avoid leaks—use pooling where allocation frequency is high.

## Development Workflow

Feature development MUST follow the spec-plan-tasks-implement checklist flow. All PRs MUST include tests for new behavior. PRs MUST pass CI gates: lint, typecheck, unit tests, integration tests. Reviews MUST verify principle compliance before approval.

## Governance

This constitution supersedes informal practices. Amendments require documented rationale and MUST be reviewed before merging. Version bumping follows semantic versioning: MAJOR for breaking changes, MINOR for new principles, PATCH for clarifications. All contributors MUST acknowledge these principles before contributing.

**Version**: 1.0.0 | **Ratified**: 2026-04-16 | **Last Amended**: 2026-04-16
