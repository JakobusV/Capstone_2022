using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class sword1 : IWeapon
{
    public string Name => "sword1";

    public string Description => "The first sword";

    public string ModelName => "sword1";

    public float Damage => 2;

    public float Knockback => 8;

    public float LingerTime => 0.2f;

    public float CoolDownTime => 0.4f;
}
