using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;
using System;
using UnityEngine.XR.Interaction.Toolkit;

public class Shooter_generator : MonoBehaviour
{
    private int _miss = 0;
    private int _success = 0;
    private int stimuls_number = 0;
    private System.Random rand = new();
    [HideInInspector] public UnityEvent<int, int> points_counter;
    [SerializeField] private TMP_Text points;
    [SerializeField] private TMP_Text balls;
    [SerializeField] private TMP_Text start_text;
    [SerializeField] private SetupConfig setup_config;
    [SerializeField] private GameObject shooter;
    [SerializeField] private DataPathCreator creator;
    [SerializeField] private GameObject statistic;
    [SerializeField] private GameObject cam;
    [SerializeField] private AudioSource click;
    [SerializeField] private AudioSource start;
    [SerializeField] private AudioSource end;
    [SerializeField] private GameObject arrow;

    private void Start()
    {
        points_counter = new();
        points_counter.AddListener(ChangePoints);
        stimuls_number = (int)setup_config.config.number_of_stimuls;
        ChangeTextOfStimuls(stimuls_number);
        StartCoroutine(StartExperiment());
    }

    private void ChangeTextOfStimuls(int number)
    {
        balls.text = $"������� ��������: {number}";
    }    
    private void ChangePoints(int miss, int success)
    {
        _miss += miss;
        _success += success;
        points.text = $"����������: {_success} \n�����������: {_miss}";
        ChangeTextOfStimuls(stimuls_number - _miss - _success);
    }
    private IEnumerator StartExperiment()
    {
        arrow.SetActive(true);
        for (int i = 0;i < 5; i++)
        {
            click.Play();
            start_text.text = $"����������� �������� ����� {5 - i} ������";
            yield return new WaitForSeconds(1f);
        }
        arrow.SetActive(false);
        start_text.gameObject.SetActive(false);
        start.Play();
        for (int i = 0; i < stimuls_number; i++)
        {
            Vector3 start_point = GenerateStartPointWithDistribution();
            float velocity = GenerateValueWithRandom(setup_config.config.stimuls_velocity[0], setup_config.config.stimuls_velocity[1]);
            velocity *= 1f + (setup_config.config.value_of_velocity_increase - 1f) * i / stimuls_number;
            bool is_false_stimuls;
            if (setup_config.config.is_false_stimuls_exists)
            {
                System.Random random = new();
                if((float)random.NextDouble() < setup_config.config.false_stimuls_percentage/100f)
                    is_false_stimuls = true;
                else
                    is_false_stimuls = false;
                //Debug.Log(setup_config.config.false_stimuls_percentage / 100f);
            }
            else is_false_stimuls = false;
            float delta_before_shoot = GenerateValueWithRandom(setup_config.config.delta_before_shoot[0], setup_config.config.delta_before_shoot[1]);
            float delta_t = GenerateValueWithRandom(setup_config.config.delta_t[0], setup_config.config.delta_t[1]);
            //Debug.Log(delta_before_shoot);
            //Debug.Log(delta_t);
            Vector3 start_point_in_global = new Vector3(start_point.z * (float)Math.Sin(Math.PI * start_point.x / 180), start_point.y, start_point.z * (float)Math.Cos(Math.PI * start_point.x / 180));
            //Debug.Log(start_point_in_global);
            //Debug.Log(velocity);
            var end_point = GenerateEndPointWithDistribution();
            //Debug.Log(end_point);
            Vector3 direction = end_point - start_point_in_global;
            var new_shooter = Instantiate(shooter, start_point_in_global, Quaternion.identity);

            SuperBallSpawnAnimator ballSpawnAnim = new_shooter.GetComponentInChildren<SuperBallSpawnAnimator>();
            ballSpawnAnim.animTime = delta_before_shoot;
            new_shooter.transform.GetChild(0).localScale = new_shooter.transform.localScale * setup_config.config.diameter_of_stimul;
            ballSpawnAnim.Init();


            new_shooter.GetComponent<Shooter_controller>().is_false_stimul = is_false_stimuls;
            new_shooter.GetComponent<Shooter_controller>().velocity = velocity;
            new_shooter.GetComponent<Shooter_controller>().delta_before_shoot = delta_before_shoot;
            new_shooter.GetComponent<Shooter_controller>().direction = direction;
            new_shooter.GetComponent<Shooter_controller>().mass_of_stimul = setup_config.config.mass_of_stimul;
            new_shooter.GetComponent<Shooter_controller>().use_gravity = setup_config.config.use_gravity;
            new_shooter.GetComponent<Shooter_controller>().diameter_of_stimul = setup_config.config.diameter_of_stimul;
            new_shooter.GetComponent<StimulDataWriter>().config_path = setup_config.full_path;
            new_shooter.GetComponent<StimulDataWriter>().data_path = creator.data_path;
            new_shooter.GetComponent<StimulDataWriter>().stimul_number = i;
            yield return new WaitForSeconds(delta_t);
        }
        end.Play();
        statistic.transform.rotation = Quaternion.identity;
        statistic.transform.position = new Vector3(0, cam.transform.position.y, 4);
        start_text.text = $"������� �������� �����: {(float)_success/stimuls_number * 100f} %";
        start_text.gameObject.SetActive(true);
    }
    private Vector3 GenerateEndPointWithDistribution()
    {
        var target_area = setup_config.config.target_area;
        float x = (float)(0 + target_area[0] / 3.5 * GenerateStdNormal());
        //Debug.Log("x : " + x);
        if(x > 0)
        {
            x = target_area[0] - x;
        }
        else x = - target_area[0] - x;
        float y = (float)((target_area[2] - target_area[1])/2 + (target_area[2] - target_area[1]) / 7 * GenerateStdNormal());
        if (y > (target_area[2] - target_area[1])/2)
        {
            y = target_area[2] - y + (target_area[2] - target_area[1]) / 2;
        }
        //else y = (target_area[2] - target_area[1]) / 2 - (y - target_area[1]);
        return new Vector3(x, y, -target_area[3]);
    }
    private Vector3 GenerateStartPointWithDistribution()
    {
        var throw_area = setup_config.config.throw_area;
        var throw_area_depth = setup_config.config.throw_area_depth;
        
        float angle =
                     (float)(0 + throw_area[0] / 3.5 * GenerateStdNormal());
        float height =
                     (float)((throw_area[2] - throw_area[1])/2 + (throw_area[2] - throw_area[1]) / 7 * GenerateStdNormal());
        float depth = GenerateValueWithRandom(throw_area_depth[0], throw_area_depth[1]);
        return new Vector3(angle, height, depth);
    }

    private float GenerateValueWithRandom(float a, float b)
    {
        return (float)(rand.NextDouble() * (b - a) + a);
    }

    private double GenerateStdNormal()
    {
        double u1 = 1.0 - rand.NextDouble();
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                     Math.Sin(2.0 * Math.PI * u2);
        return randStdNormal;
    }
}
