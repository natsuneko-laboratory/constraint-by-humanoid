# Constraint by Humanoid - Unity Editor Extension

A Unity editor extension that configures IConstraint component based on Unity Humanoid Rules.

## Requirements

- Unity 2018.4.20f1

## Installation

1. Download UnityPackage from BOOTH (Recommended)
2. Install via NPM Scoped Registry

### Download UnityPackage

You can download latest version of UnityPackage from [BOOTH](https://natsuneko.booth.pm/items/2284661).  
Extract downloaded zip package and install UnityPackage into your project.

### Install via NPM

Please add the following section to the top of the package manifest file (`Packages/manifest.json`).  
If the package manifest file already has a `scopedRegistry` section, it will bee added there.

```json
{
  "scopedRegistries": [
    {
      "name": "Mochizuki",
      "url": "https://registry.npmjs.com",
      "scopes": ["moe.mochizuki"]
    }
  ]
}
```

And the following line to the `dependencies` section:

```json
"moe.mochizuki.constraint-by-humanoid": "VERSION"
```

## How to use (Documentation / Japanese)

https://docs.mochizuki.moe/Unity/ConstraintByHumanoid/

## License

MIT by [@MikazukiFuyuno](https://twitter.com/MikazukiFuyuno) and [@6jz](https://twitter.com/6jz)
