export type SpecialtyKind =
  | 'heart'
  | 'child'
  | 'bone'
  | 'skin'
  | 'eye'
  | 'ear'
  | 'women'
  | 'brain'
  | 'tooth'
  | 'surgery'
  | 'emergency'
  | 'mind'
  | 'lung'
  | 'stomach'
  | 'kidney'
  | 'gland'
  | 'shield'
  | 'scan'
  | 'mask'
  | 'internal'
  | 'general';

export interface SpecialtyVisual {
  kind: SpecialtyKind;
  color: string;
  soft: string;
}

const visuals: Record<SpecialtyKind, SpecialtyVisual> = {
  heart: { kind: 'heart', color: '#d7263d', soft: '#fde7eb' },
  child: { kind: 'child', color: '#e0920a', soft: '#fff3d8' },
  bone: { kind: 'bone', color: '#0d8a82', soft: '#e4f6f4' },
  skin: { kind: 'skin', color: '#d23b8d', soft: '#fde7f3' },
  eye: { kind: 'eye', color: '#0e86c7', soft: '#e4f3fb' },
  ear: { kind: 'ear', color: '#7046d6', soft: '#efe8fb' },
  women: { kind: 'women', color: '#c43170', soft: '#fde7f1' },
  brain: { kind: 'brain', color: '#5746b0', soft: '#ece8fb' },
  tooth: { kind: 'tooth', color: '#2b8ed4', soft: '#e5f3fb' },
  surgery: { kind: 'surgery', color: '#c4452d', soft: '#fdece8' },
  emergency: { kind: 'emergency', color: '#e25b12', soft: '#ffefe4' },
  mind: { kind: 'mind', color: '#6a4fd0', soft: '#eeeafb' },
  lung: { kind: 'lung', color: '#148f86', soft: '#e4f6f4' },
  stomach: { kind: 'stomach', color: '#d56a1c', soft: '#fff0e3' },
  kidney: { kind: 'kidney', color: '#2a9a46', soft: '#e6f6eb' },
  gland: { kind: 'gland', color: '#b07d08', soft: '#fbf3dc' },
  shield: { kind: 'shield', color: '#6b8f16', soft: '#f1f6e3' },
  scan: { kind: 'scan', color: '#3d5afe', soft: '#eaeefe' },
  mask: { kind: 'mask', color: '#54657a', soft: '#eef1f5' },
  internal: { kind: 'internal', color: '#1d6fe0', soft: '#e7f0fd' },
  general: { kind: 'general', color: '#3d5a80', soft: '#e8eef6' },
};

const knownSpecialties: { label: string; kind: SpecialtyKind }[] = [
  { label: 'پزشکی عمومی', kind: 'general' },
  { label: 'قلب و عروق', kind: 'heart' },
  { label: 'کودکان', kind: 'child' },
  { label: 'ارتوپدی', kind: 'bone' },
  { label: 'پوست', kind: 'skin' },
  { label: 'چشم', kind: 'eye' },
  { label: 'گوش و حلق و بینی', kind: 'ear' },
  { label: 'زنان و زایمان', kind: 'women' },
  { label: 'مغز و اعصاب', kind: 'brain' },
  { label: 'دندان‌پزشکی', kind: 'tooth' },
  { label: 'اورژانس', kind: 'emergency' },
  { label: 'روان‌پزشکی', kind: 'mind' },
  { label: 'ریه', kind: 'lung' },
  { label: 'گوارش', kind: 'stomach' },
  { label: 'کلیه و مجاری ادرار', kind: 'kidney' },
  { label: 'غدد', kind: 'gland' },
  { label: 'عفونی', kind: 'shield' },
  { label: 'رادیولوژی', kind: 'scan' },
  { label: 'بیهوشی', kind: 'mask' },
  { label: 'جراحی', kind: 'surgery' },
  { label: 'داخلی', kind: 'internal' },
];

export function specialtyLabel(value: string): string {
  return value;
}

export function specialtyVisual(value: string): SpecialtyVisual {
  const text = value.replace(/\u200c/g, '');
  const exact = knownSpecialties.find((item) => item.label.replace(/\u200c/g, '') === text);
  if (exact) return visuals[exact.kind];

  const match = knownSpecialties
    .filter((item) => item.kind !== 'general')
    .sort((left, right) => right.label.length - left.label.length)
    .find((item) => text.includes(item.label.replace(/\u200c/g, '')));
  return visuals[match?.kind ?? 'general'];
}
