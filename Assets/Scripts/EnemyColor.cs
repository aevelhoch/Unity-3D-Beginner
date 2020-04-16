using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColor : MonoBehaviour
{
    public Transform player;
    private Transform m_selfTransform;
    private SkinnedMeshRenderer m_selfMesh;

    void Start()
    {
        m_selfMesh = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
        m_selfTransform = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        // get vector from john to enemy, then normalize it
        Vector3 fromJohnToEnemy = m_selfTransform.position - player.transform.position;
        fromJohnToEnemy.Normalize();

        // calculate and normalize angle between John's forward and enemy (1 if looking directly at, 0 if looking completely away)
        float alphaAngle = (Vector3.Dot(fromJohnToEnemy, player.forward) + 1)/2;

        // create a new color with transparency equal to the normalized angle
        Color alphaColor = new Color(0f, 0f, 0f, alphaAngle);
        // then apply it to the gargoyle and the flashlight
        foreach (var material in m_selfMesh.materials)
        {
            material.color = alphaColor;
        }

        // finally, we just make the model invisible when the gargoyle gets transparent enough, otherwise you can still see it when its completely clear because of lighting layers
        if (alphaAngle < 0.5f)
        {
            m_selfMesh.enabled = false;
        }
        else {
            m_selfMesh.enabled = true;
        }
    }
}
