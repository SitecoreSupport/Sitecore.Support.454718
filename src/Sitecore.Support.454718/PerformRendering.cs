using Sitecore.Diagnostics;
using Sitecore.Mvc.Exceptions;
using Sitecore.Mvc.Pipelines;
using Sitecore.Mvc.Pipelines.Response.RenderPlaceholder;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using Sitecore.Mvc.Presentation;
using System.IO;

namespace Sitecore.Support.Mvc.Pipelines.Response.RenderPlaceholder
{
    public class PerformRendering : Sitecore.Mvc.Pipelines.Response.RenderPlaceholder.PerformRendering
    {
        protected override void Render(string placeholderName, TextWriter writer, RenderPlaceholderArgs args)
        {
            foreach (Rendering current in GetRenderings(placeholderName, args))
            {
                if (current != null)
                {
                    using (this.CreateCyclePreventer(placeholderName, current))
                    {
                        this.ProcessRenderRendering(current, writer);
                    }
                }
            }
        }

        protected virtual RecursionStack CreateCyclePreventer(string placeholderName, Rendering rendering)
        {
            Assert.IsNotNull(rendering, "rendering");
            string text = rendering.Renderer.ToString();
            string text2 = placeholderName + "-" + text;
            string details = string.Concat(new object[]
            {
                "[",
                placeholderName,
                "-",
                text,
                "- {",
                rendering.UniqueId,
                "}",
                "]"
            });
            RecursionStack recursionStack = new RecursionStack("Rendering", text2, details);
            if (recursionStack.GetCount("Rendering", text2) > 1)
            {
                throw new CyclicRenderingException("A rendering has been recursively embedded within itself. Embedding trail: " + recursionStack.GetTrail("Rendering", " --> "));
            }
            return recursionStack;
        }

        protected virtual void ProcessRenderRendering(Rendering rendering, TextWriter writer)
        {
            Assert.IsNotNull(rendering, "rendering");
            Assert.IsNotNull(writer, "writer");
            PipelineService.Get().RunPipeline<RenderRenderingArgs>("mvc.renderRendering", new RenderRenderingArgs(rendering, writer));
        }
    }
}