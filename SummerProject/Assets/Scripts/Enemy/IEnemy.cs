
/**
 * Contains basic functions for Enemy objects
 */
public interface IEnemy {

    void Attack();
    void Move();
    void StopMoving();
    void Death();
    bool CanAttack();
    float CheckPlayerDistance();
    void SetWalkingAnimation();
    void ResetAnimations();

}
