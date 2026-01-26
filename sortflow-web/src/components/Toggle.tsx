type ToggleProps = {
  checked: boolean
  onChange: (v: boolean) => void
  label?: string
  disabled?: boolean
}

export default function Toggle({ checked, onChange, label, disabled }: ToggleProps) {
  return (
    <div className="toggle">
      <div
        role="switch"
        aria-checked={checked}
        tabIndex={disabled ? -1 : 0}
        className={`toggle-track ${checked ? 'on' : ''}`}
        onClick={() => !disabled && onChange(!checked)}
        onKeyDown={e => { if (!disabled && (e.key === ' ' || e.key === 'Enter')) { e.preventDefault(); onChange(!checked) } }}
      >
        <div className="toggle-thumb" />
      </div>
      {label != null && <span>{label}</span>}
    </div>
  )
}
