using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CustomParticleSystem : MonoBehaviour
{
    [Header("Configuração Base")]
    public GameObject particlePrefab;
    public int maxParticles = 100;
    public float spawnRate = 0.1f;
    private float spawnTimer;

    [Header("Modos Atuais")]
    public EmitterType currentEmitter = EmitterType.Point;
    public EvolutionType currentEvolution = EvolutionType.Gravity;

    // Enumerações que definem as variações de tragetória e nascimento
    public enum EmitterType { Point, Sphere, Box }
    public enum EvolutionType { Gravity, SineWave, Spiral, ColorFade, Shrink }

    private class Particle
    {
        public GameObject go;
        public Transform transform;
        public MeshRenderer renderer;
        public Vector3 velocity;
        public float age;
        public float lifetime;
        public Color startColor;
        public Vector3 startScale;
    }

    private List<Particle> activeParticles = new List<Particle>();
    private Queue<GameObject> particlePool = new Queue<GameObject>();

    void Start()
    {
        // Em vez de criar e destruir partículas a todo frame,
        // vai criar todas no início, deixar elas desativadas e reciclar.
        for (int i = 0; i < maxParticles; i++)
        {
            GameObject obj = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            obj.SetActive(false);
            particlePool.Enqueue(obj);
        }
    }

    void Update()
    {
        HandleInput();
        HandleBirth();
        HandleEvolutionAndDeath();
    }

    // Gerenciamento da interface do teclado
    void HandleInput()
    {
        if (Keyboard.current == null) return;

        // Troca a forma de nascimento no teclado
        if (Keyboard.current.digit1Key.wasPressedThisFrame) currentEmitter = EmitterType.Point;
        if (Keyboard.current.digit2Key.wasPressedThisFrame) currentEmitter = EmitterType.Sphere;
        if (Keyboard.current.digit3Key.wasPressedThisFrame) currentEmitter = EmitterType.Box;

        // Troca o comportamento de tragetória no teclado
        if (Keyboard.current.digit4Key.wasPressedThisFrame) currentEvolution = EvolutionType.Gravity;
        if (Keyboard.current.digit5Key.wasPressedThisFrame) currentEvolution = EvolutionType.SineWave;
        if (Keyboard.current.digit6Key.wasPressedThisFrame) currentEvolution = EvolutionType.Spiral;
        if (Keyboard.current.digit7Key.wasPressedThisFrame) currentEvolution = EvolutionType.ColorFade;
        if (Keyboard.current.digit8Key.wasPressedThisFrame) currentEvolution = EvolutionType.Shrink;
    }

    // Controla quando e quantas partículas surgem na cena.
    void HandleBirth()
    {
        spawnTimer += Time.deltaTime;

        // Só emite uma nova partícula se o tempo de intervalo passou 
        if (spawnTimer >= spawnRate && particlePool.Count > 0)
        {
            spawnTimer = 0;
            SpawnParticle();
        }
    }

    // Configura os atributos iniciais da partícula ao nascer.
    void SpawnParticle()
    {
        GameObject pObj = particlePool.Dequeue();
        pObj.SetActive(true);

        Particle p = new Particle
        {
            go = pObj,
            transform = pObj.transform,
            renderer = pObj.GetComponent<MeshRenderer>(),
            age = 0f,
            lifetime = Random.Range(2f, 4f),
            startColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f),
            startScale = Vector3.one * Random.Range(0.15f, 0.35f)
        };

        // Define ONDE a partícula nasce
        switch (currentEmitter)
        {
            case EmitterType.Point:
                // Point: Todas saem do mesmo centro
                p.transform.position = transform.position;
                break;
            case EmitterType.Sphere:
                // Spherea: Nascem em locais aleatórios dentro de uma "esfera".
                p.transform.position = transform.position + Random.insideUnitSphere * 2f;
                break;
            case EmitterType.Box:
                // Box: Nascem espalhadas por um quadrado base
                p.transform.position = transform.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
                break;
        }

        // Impulso inicial para cima
        p.velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(2f, 5f), Random.Range(-1f, 1f));
        p.transform.localScale = p.startScale;

        if (p.renderer != null && p.renderer.material != null)
            p.renderer.material.color = p.startColor;

        activeParticles.Add(p);
    }

    // Tragetória e morte das particulas
    void HandleEvolutionAndDeath()
    {
        for (int i = activeParticles.Count - 1; i >= 0; i--)
        {
            Particle p = activeParticles[i];
            p.age += Time.deltaTime;

            float lifeNormalized = p.age / p.lifetime;

            // Define como a partícula se comporta ao longo do tempo, a tragetória
            switch (currentEvolution)
            {
                case EvolutionType.Gravity:
                    // Aplica gravidade jogando a partícula para baixo
                    p.velocity += Physics.gravity * Time.deltaTime;
                    p.transform.position += p.velocity * Time.deltaTime;
                    break;

                case EvolutionType.SineWave:
                    // Usa a função Seno para criar uma trajetória ondulada
                    p.transform.position += new Vector3(Mathf.Sin(p.age * 5f) * 0.05f, p.velocity.y * Time.deltaTime, 0);
                    break;

                case EvolutionType.Spiral:
                    // Combina Seno e Cosseno para gerar uma hélice
                    p.transform.position += new Vector3(Mathf.Cos(p.age * 10f) * 0.1f, p.velocity.y * Time.deltaTime, Mathf.Sin(p.age * 10f) * 0.1f);
                    break;

                case EvolutionType.ColorFade:
                    // Move linearmente, mas a transparência diminui até sumir
                    p.transform.position += p.velocity * Time.deltaTime;
                    if (p.renderer != null)
                    {
                        Color fadeColor = p.startColor;
                        fadeColor.a = Mathf.Lerp(1f, 0f, lifeNormalized);
                        p.renderer.material.color = fadeColor;
                    }
                    break;

                case EvolutionType.Shrink:
                    // Move linearmente, mas o tamanho é interpolado até chegar a zero
                    p.transform.position += p.velocity * Time.deltaTime;
                    p.transform.localScale = Vector3.Lerp(p.startScale, Vector3.zero, lifeNormalized);
                    break;
            }

            // Verifica se a partícula atendeu aos critérios para deixar de existir
            bool shouldDie = false;

            // Morte 1: Acabou o tempo de vida total
            if (p.age >= p.lifetime)
                shouldDie = true;

            // Morte 2: Tornou-se completamente invisível
            if (currentEvolution == EvolutionType.ColorFade && p.renderer.material.color.a <= 0.05f)
                shouldDie = true;

            // Se morreu, desativa e devolve para a pool
            if (shouldDie)
            {
                p.go.SetActive(false);
                particlePool.Enqueue(p.go);
                activeParticles.RemoveAt(i);
            }
        }
    }
}