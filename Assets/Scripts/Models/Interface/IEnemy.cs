using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IEnemy
{
    float Health { get; set; }

    public void Die();

    public void TakeDamage(float Damage, float Knockback);
}
