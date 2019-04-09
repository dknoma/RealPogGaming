using System.Collections;
using UnityEngine;

public class AreaEnemyManager : MonoBehaviour {

    public static AreaEnemyManager aem;

    private int totalAreaEnemyCount;
    private GameObject enemyToBattle;
    
    private void OnEnable() {
        if (aem == null) {
            aem = this;
        } else if (aem != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        totalAreaEnemyCount = enemies.Length;
    }

    // TODO: eventtrigger - AreaChange() { update enemy pool }

    public void EnemyToBattle(GameObject enemy) {
        enemyToBattle = enemy;
    }
    
    public void DoDefeatEnemyAnimation() {
        StartCoroutine(DefeatEnemyAnimation());
    }
    
    private IEnumerator DefeatEnemyAnimation() {
        // Disable colliders and movement
        Debug.LogFormat("Defeated {0}", enemyToBattle);
        enemyToBattle.GetComponent<Collider2D>().enabled = false;
        SpriteRenderer enemyRenderer = enemyToBattle.GetComponent<SpriteRenderer>();
        int i = 1;
        while(i <= 20) {
//            enemyToBattle.SetActive(false);
            enemyRenderer.enabled = false;
            float secondsToWait = 0.7f / i;
            yield return new WaitForSeconds(secondsToWait);
            enemyRenderer.enabled = true;
            yield return new WaitForSeconds(secondsToWait);
            i += 2;
        }
        Debug.LogFormat("done");
        enemyToBattle.SetActive(false);
    }
}
