using Hax;

enum NutCrackerState {
    WALKING,
    SENTRY
}

internal class NutcrackerController : IEnemyController<NutcrackerEnemyAI> {
    bool IsSecondarySkillActive { get; set; } = false;

    public void OnMovement(NutcrackerEnemyAI enemyInstance, bool isMoving, bool isSprinting) {
        if (!isMoving && !isSprinting) return;
        enemyInstance.SetBehaviourState(NutCrackerState.WALKING);
    }

    public void UsePrimarySkill(NutcrackerEnemyAI enemyInstance) {
        if (enemyInstance.gun is not ShotgunItem shotgun) return;

        shotgun.gunShootAudio.volume = 0.25f;
        enemyInstance.FireGunServerRpc();
    }

    public void UseSecondarySkill(NutcrackerEnemyAI enemyInstance) {
        if (this.IsSecondarySkillActive) return;

        this.IsSecondarySkillActive = true;
        enemyInstance.SetBehaviourState(NutCrackerState.SENTRY);
    }

    public void ReleaseSecondarySkill(NutcrackerEnemyAI enemyInstance) {
        if (!this.IsSecondarySkillActive) return;

        this.IsSecondarySkillActive = false;
        enemyInstance.SetBehaviourState(NutCrackerState.WALKING);
    }

    public string GetPrimarySkillName(NutcrackerEnemyAI enemyInstance) => enemyInstance.gun is null ? "" : "Fire";

    public string GetSecondarySkillName(NutcrackerEnemyAI _) => "(HOLD) Sentry mode";
}
