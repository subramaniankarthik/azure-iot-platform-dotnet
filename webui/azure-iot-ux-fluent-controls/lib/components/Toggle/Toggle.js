"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const React = require("react");
const classNames = require("classnames/bind");
const styled_components_1 = require("styled-components");
const Attributes_1 = require("../../Attributes");
const Common_1 = require("../../Common");
const css = classNames.bind(require('./Toggle.module.scss'));
const StyledToggleOnButton = styled_components_1.default(Attributes_1.Elements.button) `
    &&&&&& {
        background-color: ${(props) => props.theme.colorBgBtnPrimaryRest};
        &:hover {
            background-color: ${(props) => props.theme.colorBgBtnPrimaryHover};
        }
    }
`;
const StyledToggleOnSwitch = styled_components_1.default(Attributes_1.Elements.div) `
    &&&& {
        background-color: ${(props) => props.theme.colorTextBtnPrimaryRest};
    }
`;
/**
 * Toggle button that is an on or off state
 *
 * @param props Control properties (defined in `ToggleProps` interface)
 */
exports.Toggle = (props) => {
    const containerClassName = css('toggle', {
        'toggle-on': props.on,
        'disabled': props.disabled
    });
    const onClick = (event) => {
        if (!props.disabled && props.onChange) {
            props.onChange(!props.on);
        }
    };
    const tabIndex = props.disabled ? -1 : null;
    const label = props.on ? props.onLabel : props.offLabel;
    const ToggleButtonProxy = props.on ? StyledToggleOnButton : Attributes_1.Elements.button;
    const ToggleSwitchProxy = props.on ? StyledToggleOnSwitch : Attributes_1.Elements.div;
    return (React.createElement(Attributes_1.Elements.div, { className: containerClassName, attr: props.attr.container },
        React.createElement(ToggleButtonProxy, { type: 'button', className: css('toggle-button'), onClick: onClick, tabIndex: tabIndex, name: props.name, autoFocus: props.autoFocus, methodRef: props.autoFocus && Common_1.autoFocusRef, 
            // https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/ARIA_Techniques/Using_the_switch_role
            // the switch role represents the states "on" and "off."
            role: 'switch', "aria-checked": props.on, attr: props.attr.button }),
        React.createElement(ToggleSwitchProxy, { className: css('toggle-switch'), attr: props.attr.switch }),
        React.createElement(Attributes_1.Elements.div, { className: css('toggle-label'), attr: props.attr.text }, label)));
};
exports.Toggle.defaultProps = {
    name: undefined,
    onChange: undefined,
    onLabel: 'On',
    offLabel: 'Off',
    attr: {
        container: {},
        button: {},
        border: {},
        switch: {},
        text: {},
    }
};
exports.default = exports.Toggle;
