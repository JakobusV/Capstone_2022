using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IWeapon
{
    string Name { get; }
    string Description { get; }
    string ModelName { get; }
    float Damage { get; }
    float Knockback { get; }
    float LingerTime { get; }
    float CoolDownTime { get; }
}
