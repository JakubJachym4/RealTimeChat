﻿import { Popup } from './Popups.js'

export const LoginModule = (input) => {
    const { inputID, isMouseOver, cssClassName, innerText, placeholder } = input
    let textBox = document.getElementById(inputID)
    if (isMouseOver == true) {
        let newSpan = document.createElement('span')
        newSpan.innerText = innerText
        textBox['placeholder'] = ''
        textBox.parentElement.appendChild(newSpan)
        newSpan.classList.add(cssClassName)
        newSpan.classList.add('default-font')
    }
    else {
        let span = document.querySelector(`.${cssClassName}`)
        if (span) {
            span.remove()
            textBox['placeholder'] = placeholder
        }
    }
}

export const PaperPlane = () => {
    let PaperPlane = document.getElementsByClassName('im-paperplane')
    PaperPlane = PaperPlane.length == 1 ? PaperPlane[0] : null

    if (PaperPlane) {
        if (PaperPlane.classList.contains('flying-plane') == false) {
            PaperPlane.classList.add('flying-plane')
            PaperPlane.classList.remove('invisible')
            setTimeout(() => {
                console.log('abc')
                PaperPlane.classList.remove('flying-plane')
                PaperPlane.classList.add('invisible')
            }, 700)
        }
    }
}

export const ErrorModalShow = (message,show) => {
    let popup = new Popup(message, 'error')
    popup.ShowPopup()
    popup.RegisterCloseEvent()
}


