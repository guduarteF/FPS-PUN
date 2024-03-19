using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; // acessar arquivos , deletar arquivos , modificiar arquivos do sistema
using System.Runtime.Serialization.Formatters.Binary; // permite serializar e deserializar as classes em arquivos em tempo de execução

public class Data : MonoBehaviour
{
    
    public static void SaveProfile (ProfileData t_profile)
    {
        string path = Application.persistentDataPath + "/profile.dt";

    }
}
