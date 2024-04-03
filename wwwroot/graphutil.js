function createAlgorithm(name, alg) {
    try
    {
        let _ = eval(name)
        if(_)
            return true
    }
    catch {}
    try
    {
        eval(alg)
    }
    catch(e)
    {
        console.log("Failed to add algorithm")
        console.log(alg)
        console.error(e)
        return false
    }
    let script = document.createElement('script')
    script.appendChild(document.createTextNode(alg))
    document.head.appendChild(script)
    return true
}

async function copy(text)
{
    await navigator.clipboard.writeText(text)
}