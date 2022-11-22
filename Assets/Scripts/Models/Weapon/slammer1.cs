using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class slammer1 : IWeapon
{
    public string Name => "slammer1";

    public string Description => "It's a hammer for slamming.";

    public string ModelName => "slammer1";

    public float Damage => 1;

    public float Knockback => 64;

    public float LingerTime => 0.3f;

    public float CoolDownTime => 0.8f;
}
