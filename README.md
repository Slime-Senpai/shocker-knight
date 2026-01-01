# shocker-knight
A Hollow Knight mod for PiShock integration

> [!IMPORTANT]
> For legal reasons, this tool only exists for research purposes.
> Do not attempt to use it, especially not on yourself!


## Installing ?

This mod requires Satchel to work.
It installs like any mod and all configuration is done in game.
Configure the settings to your liking (or to your disliking considering the kind of config?)
Don't forget to press the button at the bottom to toggle the API settings and configure those.

## How does it work ?

Whenever you get damaged, a random value will be selected between
your min and max for intensity and duration.

Those random values will be multiplied by multipliers depending on the amount
of damage and the current health.

To be specific, the lower your health, the higher the multiplier for intensity.
If you have blue health, you can actually lower the multiplier below 1.

For duration, amount of damage you take will be the multiplier (so x2 for a 2 masks hit).

If you want to spice things up, I recommend turning on Overchage as that means
that the multipliers can push the random values above the max (and below the min)
for that extra spicy when you get double hit (or even quadruple hit with overcharm!).
