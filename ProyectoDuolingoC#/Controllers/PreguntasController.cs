using Microsoft.AspNetCore.Mvc;
using ProyectoDuolingoC_.Models;
using ProyectoDuolingoC_.Repositories;

namespace ProyectoDuolingoC_.Controllers
{
    public class PreguntasController : Controller
    {
        RepositoryPreguntas repo;
        RepositoryLecciones repoLec;
        public PreguntasController(RepositoryPreguntas repo, RepositoryLecciones repoLec)
        {
            this.repo = repo;
            this.repoLec = repoLec;
        }
        public async Task<IActionResult> Menu(int id)
        {
            List<Pregunta> preg = await this.repo.VerPregunta(id);
            return View(preg);
        }
        public async Task<IActionResult> Victoria(int leccionId)
        {

            Leccion leccion = await this.repoLec.VerContenido(leccionId);
            return View(leccion);
        }

        public async Task<IActionResult> Preguntas(int id, int index = 0)
        {
            List<Pregunta> preguntas = await this.repo.VerPregunta(id);

            if (preguntas == null || index >= preguntas.Count)
            {
                return RedirectToAction("Victoria", new { leccionId = id });
            }

            Pregunta preguntaActual = preguntas[index];
            List<OpcionRespuesta> opciones = await this.repo.VerOpciones(preguntaActual.PreguntaID);

            ViewData["OPCIONES"] = opciones;
            ViewData["INDEX_ACTUAL"] = index;
            ViewData["TOTAL_PREGUNTAS"] = preguntas.Count;
            ViewData["PROGRESO_PORCENTAJE"] = (int)((double)index / preguntas.Count * 100);

            return View(preguntaActual);
        }
        [HttpPost]
        public async Task<IActionResult> VerificarRespuesta(int PreguntaID, string RespuestaAlumno, int IndexActual, int LeccionID)
        {
            if (string.IsNullOrWhiteSpace(RespuestaAlumno))
            {
                TempData["MENSAJE"] = "Por favor, selecciona o escribe una respuesta.";
                TempData["TIPO_MENSAJE"] = "error";
                return RedirectToAction("Preguntas", new { id = LeccionID, index = IndexActual });
            }

            Pregunta pregunta = await this.repo.VerPreguntaPorId(PreguntaID);

            bool esAcierto = false;

            if (pregunta.TipoPregunta == "CompletarCodigo" || pregunta.TipoPregunta == "VerdaderoFalso")
            {
                string textoAlumno = RespuestaAlumno.Trim().Replace(" ", "");
                string textoCorrecto = pregunta.RespuestaCorrecta!.Trim().Replace(" ", "");

                if (textoAlumno.Equals(textoCorrecto, StringComparison.OrdinalIgnoreCase))
                {
                    esAcierto = true;
                }
            }
            else
            {
                if (int.TryParse(RespuestaAlumno, out int idOpcionPulsada))
                {
                    if (idOpcionPulsada == pregunta.OpcionCorrectaID)
                    {
                        esAcierto = true;
                    }
                }
            }

            if (esAcierto)
            {
                return RedirectToAction("Preguntas", new { id = LeccionID, index = IndexActual + 1 });
            }
            else
            {
                TempData["MENSAJE"] = "Respuesta incorrecta. ¡Piénsalo bien e inténtalo de nuevo!";
                TempData["TIPO_MENSAJE"] = "error";

                return RedirectToAction("Preguntas", new { id = LeccionID, index = IndexActual });
            }
        }
        public async Task<IActionResult> VerPreguntas(int idLec)
        {
            List<Pregunta> preg = await this.repo.VerPreguntasPorLeccion(idLec);
            return View(preg);
        }
        public async Task<IActionResult> Update(int id)
        {
            Pregunta preg = await this.repo.VerPreguntaPorId(id);
            if (preg.TipoPregunta != "CompletarCodigo")
            {
                List<OpcionRespuesta> opc = await this.repo.VerOpciones(id);
                ViewData["OPCIONES"] = opc;
            }
            return View(preg);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Pregunta preguntaModificada)
        {

                await this.repo.ActualizarPregunta(preguntaModificada);

                TempData["MENSAJE"] = "¡Pregunta actualizada con éxito!";
                TempData["TIPO_MENSAJE"] = "success";

                return RedirectToAction("VerPreguntas", new { idLec = preguntaModificada.LeccionID });
        }
        public IActionResult Create(int leccionId)
        {
            Pregunta nuevaPregunta = new Pregunta
            {
                LeccionID = leccionId
            };
            return View(nuevaPregunta);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Pregunta pregunta)
        {

            if (pregunta.TipoPregunta == "OpcionMultiple")
            {
                pregunta.RespuestaCorrecta = "PorDefinir";
            }

            await this.repo.CrearPregunta(pregunta);

            if(pregunta.TipoPregunta == "OpcionMultiple") {
                return RedirectToAction("VerOpciones", new { id = pregunta.PreguntaID });
            }

            return RedirectToAction("VerPreguntas", new { idLec = pregunta.LeccionID });
        }
        [HttpGet]
        public async Task<IActionResult> VerOpciones(int id) 
        {
            List<OpcionRespuesta> opciones = await this.repo.VerOpciones(id);

            ViewData["PreguntaID"] = id;

            return View(opciones);
        }

        [HttpPost]
        public async Task<IActionResult> AddOption(int PreguntaID, string TextoOpcion)
        {
            if (!string.IsNullOrWhiteSpace(TextoOpcion))
            {
                await this.repo.InsertarOpcion(PreguntaID, TextoOpcion);
            }

            return RedirectToAction("VerOpciones", new { id = PreguntaID });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOption(int OpcionID, int PreguntaID)
        {
            await this.repo.EliminarOpcion(OpcionID);
            return RedirectToAction("VerOpciones", new { id = PreguntaID });
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            int idLeccion = (await this.repo.VerPreguntaPorId(id)).LeccionID;
            await this.repo.Delete(id);
            return RedirectToAction("VerPreguntas", new { idLec = idLeccion });
        }
    }
}
